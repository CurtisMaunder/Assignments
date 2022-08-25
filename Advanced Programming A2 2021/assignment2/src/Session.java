import java.sql.Connection;

public class Session {
    private String role;
    private Connection conn;
    private int staffID;
    public Session(String role, int staffID, Connection conn){
        this.role = role;
        this.staffID = staffID;
        this.conn = conn;
    }

    public String getRole(){
        return role;
    }
    
    public Connection getConnection(){
        return conn;
    }

    public int getStaffID(){
        return staffID;
    }

    public void setRole(String role){
        this.role = role;
    }

    public void setStaffID(int staffID){
        this.staffID = staffID;
    }
}
