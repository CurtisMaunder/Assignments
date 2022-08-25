import java.sql.*;
import java.sql.SQLException;
import java.text.SimpleDateFormat;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Date;

public class DatabaseManager {
    static void newResidentToDatabase(Resident resident, Session session, int bedID) throws SQLException{
        Connection conn = session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("INSERT INTO Residents VALUES(?,?,?,?,?,?,?,?)");
        preparedStatement.setString(1, null);
        preparedStatement.setString(2, resident.getFirstName());
        preparedStatement.setString(3, resident.getLastName());
        preparedStatement.setString(4, resident.getSex());
        Date admissionDate = new Date();
        SimpleDateFormat formatter = new SimpleDateFormat("dd/MM/YYY");
        String strAdmissionDate = formatter.format(admissionDate);
        preparedStatement.setString(5, strAdmissionDate);
        preparedStatement.setString(6, null);
        PreparedStatement stmt = conn.prepareStatement("SELECT RoomID FROM Beds WHERE ID = ?");
        stmt.setInt(1, bedID);
        ResultSet rs = stmt.executeQuery();
        preparedStatement.setInt(7, rs.getInt("RoomID"));
        preparedStatement.setInt(8, bedID);
        preparedStatement.executeUpdate();
        
        PreparedStatement sstmt = conn.prepareStatement("SELECT * FROM Residents WHERE BedID = ?");
        sstmt.setInt(1, bedID);
        ResultSet srs = sstmt.executeQuery();
        resident.setID(srs.getInt("ID"));
    }

    static ArrayList<Resident> getResidentsFromDB(Session session) throws SQLException{
        ArrayList<Resident> residents = new ArrayList<Resident>();
        Connection conn = session.getConnection();
        ResultSet rs;
        Statement stmt = conn.createStatement();

        rs = stmt.executeQuery("SELECT * FROM Residents WHERE BedID != 0");
        while(rs.next()){
            int id = rs.getInt("ID");
            String fName = rs.getString("FirstName");
            String lName = rs.getString("LastName");
            String sex = rs.getString("Sex");
            int bedID = rs.getInt("BedID");
            residents.add(new Resident(fName, lName, id, sex, bedID));
        }

        return residents;
    }

    static ArrayList<Staff> getStaffFromDB(Session session) throws SQLException{
        ArrayList<Staff> staff = new ArrayList<Staff>();
        Connection conn = session.getConnection();
        ResultSet rs;
        Statement stmt = conn.createStatement();

        rs = stmt.executeQuery("SELECT * FROM Staff WHERE StaffID != 0");
        while(rs.next()){
            int id = rs.getInt("StaffID");
            String role = rs.getString("Role");
            String fName = rs.getString("FirstName");
            String lName = rs.getString("LastName");
            String phone = rs.getString("Phone");
            if(role.equals("D"))
                staff.add(new Doctor(fName + " " + lName, id, role, phone));
            else if(role.equals("N"))
                staff.add(new Nurse(fName + " " + lName, id, role, phone));
        }

        return staff;
    }

    static void updateResident(Resident resident, Session session, int bedID) throws SQLException{
        Connection conn = session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("UPDATE Residents SET BedID = ? WHERE ID = ?");
        preparedStatement.setInt(1, bedID);
        preparedStatement.setInt(2, resident.getID());
        preparedStatement.executeUpdate();
    }

    static ArrayList<Prescription> getPrescriptions(int residentID) throws SQLException{
        ArrayList<Prescription> prescriptions = new ArrayList<Prescription>();
        Connection conn = App.session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("SELECT * FROM Prescriptions WHERE PatientID = ?");
        preparedStatement.setInt(1, residentID);
        ResultSet rs = preparedStatement.executeQuery();
        while(rs.next()){
            int id = rs.getInt("ID");
            int patientID = rs.getInt("PatientID");
            int medicineID = rs.getInt("MedicineID");
            String dose = rs.getString("Dose");
            String interval = rs.getString("Interval");
            String datePrescribed = rs.getString("datePrescribed");
            String notes = rs.getString("Notes");
            prescriptions.add(new Prescription(id, medicineID, patientID, dose, interval, datePrescribed, notes));
        }
        return prescriptions;
    }

    static ArrayList<Medicine> getMedicines() throws SQLException{
        ArrayList<Medicine> medicines = new ArrayList<Medicine>();

        Connection conn = App.session.getConnection();
        Statement statement = conn.createStatement();
        ResultSet rs = statement.executeQuery("SELECT * FROM Medicines");
        while(rs.next()){
            int id = rs.getInt("ID");
            String name = rs.getString("Name");
            medicines.add(new Medicine(id, name));
        }

        return medicines;
    }

    static ArrayList<Shift> getShifts() throws SQLException{
        ArrayList<Shift> shifts = new ArrayList<Shift>();

        Connection conn = App.session.getConnection();
        Statement statement = conn.createStatement();
        ResultSet rs = statement.executeQuery("SELECT * FROM Shifts");
        while(rs.next()){
            int id = rs.getInt("ID");
            String day = rs.getString("Day");
            String timeSlot = rs.getString("TimeSlot");
            int staffID = rs.getInt("StaffID");
            shifts.add(new Shift(id, day, timeSlot, staffID));
        }

        return shifts;
    }

    static int addStaff(String firstName, String lastName, String role, String phone) throws SQLException{
        Connection conn = App.session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("INSERT INTO Staff VALUES(?,?,?,?,?,?)");
        preparedStatement.setString(1, null);
        preparedStatement.setString(2, role);
        preparedStatement.setString(3, firstName);
        preparedStatement.setString(4, lastName);
        preparedStatement.setString(5, phone);
        preparedStatement.setString(6, "password123");
        preparedStatement.executeUpdate();

        Statement stmt = conn.createStatement();
        ResultSet rs = stmt.executeQuery("SELECT seq FROM sqlite_sequence WHERE name = 'Staff'");
        
        return rs.getInt("seq");
    }

    static void updateStaffPhone(String newPhone, int id) throws SQLException{
        Connection conn = App.session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("UPDATE Staff SET Phone = ? WHERE StaffID = ?");
        preparedStatement.setString(1, newPhone);
        preparedStatement.setInt(2, id);
        preparedStatement.executeUpdate();
    }

    static void updateStaffPassword(String newPass, int id) throws SQLException{
        Connection conn = App.session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("UPDATE Staff SET Password = ? WHERE StaffID = ?");
        preparedStatement.setString(1, newPass);
        preparedStatement.setInt(2, id);
        preparedStatement.executeUpdate();
    }

    static int addPrescription(int medicineID, int patientID, String dose, String interval, String datePrescribed, String notes) throws SQLException{
        Connection conn = App.session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("INSERT INTO Prescriptions VALUES(?,?,?,?,?,?,?)");
        preparedStatement.setString(1, null);
        preparedStatement.setInt(2, patientID);
        preparedStatement.setInt(3, medicineID);
        preparedStatement.setString(4, dose);
        preparedStatement.setString(5, interval);
        preparedStatement.setString(6, datePrescribed);
        preparedStatement.setString(7, notes);
        preparedStatement.executeUpdate();

        Statement stmt = conn.createStatement();
        ResultSet rs = stmt.executeQuery("SELECT seq FROM sqlite_sequence WHERE name = 'Prescriptions'");
        
        return rs.getInt("seq");
    }

    static void logEvent(int staffID, String event) throws SQLException{
        LocalDateTime date = LocalDateTime.now();
        DateTimeFormatter dateFormat = DateTimeFormatter.ofPattern("dd/MM/YYYY");
        String loggedDate = date.format(dateFormat);
        dateFormat = DateTimeFormatter.ofPattern("h:m:sa");
        String loggedTime = date.format(dateFormat);
        Connection conn = App.session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("INSERT INTO EventLog VALUES(?,?,?,?,?,?)");
        preparedStatement.setString(1, null);
        preparedStatement.setString(2, loggedDate);
        preparedStatement.setString(3, loggedTime);
        preparedStatement.setInt(4, staffID);
        preparedStatement.setString(5, null);
        preparedStatement.setString(6, event);
        preparedStatement.executeUpdate();

    }

    static void logEvent(int staffID, int residentID, String event) throws SQLException{
        LocalDateTime date = LocalDateTime.now();
        DateTimeFormatter dateFormat = DateTimeFormatter.ofPattern("dd/MM/YYYY");
        String loggedDate = date.format(dateFormat);
        dateFormat = DateTimeFormatter.ofPattern("h:mm:ssa");
        String loggedTime = date.format(dateFormat);
        Connection conn = App.session.getConnection();
        PreparedStatement preparedStatement = conn.prepareStatement("INSERT INTO EventLog VALUES(?,?,?,?,?,?)");
        preparedStatement.setString(1, null);
        preparedStatement.setString(2, loggedDate);
        preparedStatement.setString(3, loggedTime);
        preparedStatement.setInt(4, staffID);
        preparedStatement.setInt(5, residentID);
        preparedStatement.setString(6, event);
        preparedStatement.executeUpdate();
    }

    static boolean validateLogin(String id, String password, Connection conn) throws SQLException{
        PreparedStatement preparedStatement = conn.prepareStatement("SELECT Password FROM Staff WHERE StaffID = ?");
        preparedStatement.setString(1, id);
        ResultSet rs = preparedStatement.executeQuery();
        if(rs.getString("Password").equals(password))
            return true;
        else
            return false;
    }

    static String getLoginRole(String id, Connection conn){
        String role = "";
        PreparedStatement preparedStatement;
        try {
            preparedStatement = conn.prepareStatement("SELECT Role FROM Staff WHERE StaffID = ?");
            preparedStatement.setString(1, id);
            ResultSet rs = preparedStatement.executeQuery();
            role = rs.getString("Role");
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return role;
    }
}
