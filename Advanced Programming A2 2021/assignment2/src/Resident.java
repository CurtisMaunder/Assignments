import java.sql.SQLException;
import java.util.ArrayList;

public class Resident {
    private String firstName;
    private String lastName;
    private int id;

    private String sex;
    private int bed;
    private ArrayList<Prescription> prescriptions;

    public Resident(String firstName, String lastName, int id, String sex, int bed){
        this.firstName = firstName;
        this.lastName = lastName;
        this.id = id;
        this.sex = sex;
        this.bed = bed;
        try {
            prescriptions = DatabaseManager.getPrescriptions(id);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public String getFirstName(){
        return firstName;
    }

    public void setFirstName(String firstName){
        this.firstName = firstName;
    }

    public String getLastName(){
        return lastName;
    }

    public void setLastName(String lastName){
        this.lastName = lastName;
    }

    public String getSex(){
        return sex;
    }

    public void setSex(String sex){
        this.sex = sex;
    }

    public int getID(){
        return id;
    }

    public void setID(int id){
        this.id = id;
    }

    public int getBedID(){
        return bed;
    }

    public void setBedID(int bed){
        this.bed = bed;
    }

    public ArrayList<Prescription> getPrescriptions(){
        return prescriptions;
    }

    @Override
    public String toString(){
        return firstName + " " + lastName;
    }

    public void addPrescription(Prescription prescription){
        prescriptions.add(prescription);
    }
}
