import java.sql.*;
import java.util.ArrayList;

import javafx.application.Application;
import javafx.geometry.Orientation;
import javafx.scene.Scene;
import javafx.scene.control.Tab;
import javafx.scene.control.TabPane;
import javafx.scene.layout.FlowPane;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

public class App extends Application{
    static ArrayList<Staff> staff = new ArrayList<Staff>();
    static ArrayList<Resident> residents = new ArrayList<Resident>();
    static CareHome careHome = new CareHome("CareHome", "CareHome");
    static int runescape;
    static Session session;
    @Override
    public void start(Stage stage) throws SQLException{
        //Connect to SQL database
        Connection conn = connectToDatabase("jdbc:sqlite:../res/CareHome.db");

        //Make a default scene
        GridPane defaultPane = new GridPane();
        Scene defaultScene = new Scene(new StackPane(defaultPane), 1280, 720);
        stage.setScene(defaultScene);
        stage.show();

        //Login
        session = Login.login(conn, stage);

        careHome.addMedicines();

        //Generate all the data
        //Generating objects
        //Staff
        staff = DatabaseManager.getStaffFromDB(session);
        //Residents
        residents = DatabaseManager.getResidentsFromDB(session);
        //Wards
        Statement stmt = conn.createStatement();
        ResultSet rs = stmt.executeQuery("SELECT * FROM Wards");
        PreparedStatement roomStatement = conn.prepareStatement("SELECT * FROM Rooms WHERE WardID = ?");
        PreparedStatement bedStatement = conn.prepareStatement("SELECT * FROM Beds WHERE RoomID = ?");
        ResultSet roomSet;
        ResultSet bedSet;
        while(rs.next()){
            Ward ward = new Ward("Ward" + rs.getString("ID"), rs.getInt("ID"));
            //Get rooms
            roomStatement.setInt(1, rs.getInt("ID"));
            roomSet = roomStatement.executeQuery();
            while(roomSet.next()){
                Room room = new Room(roomSet.getInt("ID"));
                //Get beds
                bedStatement.setInt(1, roomSet.getInt("ID"));
                bedSet = bedStatement.executeQuery();
                while(bedSet.next()){
                    //We set a dummy resident with an ID of 0 if the bed is vacant
                    Bed bed = new Bed(bedSet.getInt("ID"), new Resident("0", "0", 0, "0", 0), session);
                    for (Resident r : residents) {
                        if(r.getBedID() == bedSet.getInt("ID")){
                            bed = new Bed(bedSet.getInt("ID"), r, session);
                        }
                    }
                    room.addBed(bed);
                }
                ward.addRoom(room);
            }
            careHome.addWard(ward);
        }

        //Set pane out
        TabPane tabPane = new TabPane();

        //Creating main GUI
        for (Ward ward : careHome.wards) {
            FlowPane pane = new FlowPane(Orientation.VERTICAL,5,5);
            pane.getChildren().add(ward.wardPane);
            Tab wardTab = new Tab(ward.getName(), pane);
            tabPane.getTabs().add(wardTab);
        }

        if(session.getRole().equals("M")){
            VBox staffPane = Administration.populateStaffTab(staff);
            Tab staffTab = new Tab("Staff", staffPane);
            tabPane.getTabs().add(staffTab);
        }

        Scene scene = new Scene(new StackPane(tabPane), 1280, 720);
        
        stage.setTitle("Able Care Home");
        stage.setScene(scene);
    }

    public static void main(String[] args) throws Exception {
        launch();
    }

    public Connection connectToDatabase(String url) throws SQLException{
        Connection conn;
        return conn = DriverManager.getConnection(url); 
    }
}
