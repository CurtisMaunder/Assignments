import java.sql.SQLException;
import java.util.ArrayList;

public class CareHome {
    private String name;
    private String address;
    public ArrayList<Ward> wards;
    public ArrayList<Medicine> medicines;
    public ArrayList<Shift> shifts;

    public CareHome(String name, String address){
        this.name = name;
        this.address = address;
        wards = new ArrayList<Ward>();
        medicines = new ArrayList<Medicine>();
    }

    public void addMedicines(){
        try {
            medicines = DatabaseManager.getMedicines();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public void addShifts(){
        try {
            shifts = DatabaseManager.getShifts();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public void addWard(Ward ward){
        wards.add(ward);
    }
}
