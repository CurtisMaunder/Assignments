import java.sql.SQLException;
import java.time.LocalDateTime;
import java.util.ArrayList;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.HBox;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

public class Administration {
    final static int TEXT_FIELD_WIDTH = 100;
    //This method contains GUI generation for the staff tab
    //Its inside the administration class file because it only needs to be rendered when the admin is logged in
    static public VBox populateStaffTab(ArrayList<Staff> staff){
        //Initiaying necessary GUI containers
        VBox staffPane = new VBox();
        VBox doctorPane = new VBox();
        VBox nursePane = new VBox();

        //List doctors
        staffPane.getChildren().add(new Label("Doctors: "));
        for(Staff s : staff){
            if(s.getRole().equals("D"))
                doctorPane.getChildren().add(s.hBox);
        }
        staffPane.getChildren().add(doctorPane);

        //List nurses
        staffPane.getChildren().add(new Label("Nurses: "));
            for(Staff s : staff){
                if(s.getRole().equals("N"))
                    nursePane.getChildren().add(s.hBox);
            }
        staffPane.getChildren().add(nursePane);

        //Add staff form
        staffPane.getChildren().add(new Label("Add new staff member: "));

        ComboBox<String> roleBox = new ComboBox<String>();
        roleBox.getItems().addAll("D", "N");
        staffPane.getChildren().add(roleBox);

        staffPane.getChildren().add(new Label("First Name: "));
        TextField firstNameTextField = new TextField();
        firstNameTextField.setMaxWidth(TEXT_FIELD_WIDTH);
        staffPane.getChildren().add(firstNameTextField);

        staffPane.getChildren().add(new Label("Last Name: "));
        TextField lastNameTextField = new TextField();
        lastNameTextField.setMaxWidth(TEXT_FIELD_WIDTH);
        staffPane.getChildren().add(lastNameTextField);

        staffPane.getChildren().add(new Label("Phone: "));
        TextField phoneTextField = new TextField();
        phoneTextField.setMaxWidth(TEXT_FIELD_WIDTH);
        staffPane.getChildren().add(phoneTextField);

        Button addStaffBtn = new Button("Add staff");

        addStaffBtn.setOnAction(new EventHandler<ActionEvent>(){
            @Override
            public void handle(ActionEvent e){
                String firstName = firstNameTextField.getText();
                String lastName = lastNameTextField.getText();
                String phone = phoneTextField.getText();
                String role = roleBox.getValue();

                try {
                    //Adds new staff member to both the local object and to the database
                    int id = DatabaseManager.addStaff(firstName, lastName, role, phone);
                    if(role.equals("D")){
                        App.staff.add(new Doctor(firstName + " " + lastName, id, role, phone));
                        doctorPane.getChildren().add(App.staff.get(App.staff.size() -1).hBox);
                        DatabaseManager.logEvent(App.session.getStaffID(), "Added new doctor");
                    }else if(role.equals("N")){
                        App.staff.add(new Nurse(firstName + " " + lastName, id, role, phone));
                        nursePane.getChildren().add(App.staff.get(App.staff.size() -1).hBox);
                        DatabaseManager.logEvent(App.session.getStaffID(), "Added new nurse");
                    }
                } catch (SQLException e1) {
                    e1.printStackTrace();
                }
            }
        });

        staffPane.getChildren().add(addStaffBtn);
        return staffPane;
    }

    /*
    static public VBox populateShifts(){
        VBox shiftPane = new VBox();
        shiftPane.getChildren().add(new Label("Shifts: "));
        for (Shift s : App.careHome.shifts){
            shiftPane.getChildren().add();
        }
        return shiftPane;
    }
    */

    //This method contains GUI generation for the staff tab
    //Its inside the administration class file because it only needs to be rendered when the admin is logged in
    static Resident moveResident(Session session) throws SQLException{
        //First we create a dummy resident
        Resident resident = new Resident("0", "0", 0, "0", 0);
        //This is a modal window so a stage needs to be created
        Stage modalStage = new Stage();

        //Checks if the modal window loses focus, and if so closes it
        //This is to prevent multiple windows opening
        modalStage.focusedProperty().addListener((ov, onHidden, onShown) -> {
            if(!modalStage.isFocused())
                modalStage.close();
        });

        GridPane moveResidentPane = new GridPane();
        moveResidentPane.setAlignment(Pos.CENTER);
        Scene moveResidentScene = new Scene(new StackPane(moveResidentPane), 400, 600);

        Label infoLabel = new Label("Move which resident to this bed:");
        moveResidentPane.add(infoLabel, 0, 3);
        ComboBox<Resident> residentComboBox = new ComboBox<Resident>();
        for (Resident r : App.residents) {
            residentComboBox.getItems().add(r);
        }

        moveResidentPane.add(residentComboBox, 0, 4);
        modalStage.setScene(moveResidentScene);

        Button btn = new Button("Move Resident");

        btn.setOnAction(new EventHandler<ActionEvent>(){
            @Override
            public void handle(ActionEvent e){
                Resident tmpResident = residentComboBox.getSelectionModel().getSelectedItem();
                //Remove resident from old bed
                for (Ward ward : App.careHome.wards) {
                    for(Room room : ward.rooms){
                        for(Bed bed : room.beds){
                            if(bed.getResident().getID() == tmpResident.getID()){
                                //Setting the old bed's resident to a dummy resident to reset functionality
                                bed.setResident(new Resident("0", "0", 0, "0", 0));
                                bed.updateBed(bed.getResident());
                                break;
                            }
                        }
                    }
                }
                //Add resident to new bed
                resident.setFirstName(tmpResident.getFirstName());
                resident.setLastName(tmpResident.getLastName());
                resident.setID(tmpResident.getID());
                resident.setSex(tmpResident.getSex());
                resident.setBedID(tmpResident.getBedID());
                modalStage.close();
            }
        });

        moveResidentPane.add(btn, 2, 0);
        modalStage.showAndWait();
        
        return resident;
    }

    static public Resident modal(){
        //For assigning resident to vacant bed or for viewing resident information
        Stage modalStage = new Stage();
        //Closing the stage when it isnt focused, to prevent vacant beds being reoccupied or many windows building up over a session
        modalStage.focusedProperty().addListener((ov, onHidden, onShown) -> {
            if(!modalStage.isFocused())
                modalStage.close();
        });

        Resident resident = new Resident("0", "0", 0, "0", 0);

        GridPane residentPane = new GridPane();
        residentPane.setAlignment(Pos.CENTER);
        residentPane.setHgap(10);
        residentPane.setVgap(10);
        residentPane.setPadding(new Insets(25,25,25,25));
        Scene modalScene = new Scene(new StackPane(residentPane), 400, 600);

        Label fNameLabel = new Label("First Name:");
        residentPane.add(fNameLabel, 0, 1);

        TextField fNameTextField = new TextField();
        residentPane.add(fNameTextField, 1, 1);

        Label lNameLabel = new Label("Last Name");
        residentPane.add(lNameLabel, 0, 2);

        TextField lNameTextField = new TextField();
        residentPane.add(lNameTextField, 1, 2);

        Label sex = new Label("Sex");
        residentPane.add(sex, 0, 3);

        ComboBox<String> sexComboBox = new ComboBox<String>();
        sexComboBox.getItems().addAll(
            "Male",
            "Female"
        );

        sexComboBox.setValue("Male");
        residentPane.add(sexComboBox, 1, 3);

        Button btn = new Button("Add Resident");

        btn.setOnAction(new EventHandler<ActionEvent>(){
            @Override
            public void handle(ActionEvent e){
                resident.setFirstName(fNameTextField.getText());
                resident.setLastName(lNameTextField.getText());
                resident.setSex(sexComboBox.getValue());
                resident.setBedID(0);
                modalStage.close();
            }
        });

        HBox hbBtn = new HBox(10);
        hbBtn.setAlignment(Pos.BOTTOM_RIGHT);
        hbBtn.getChildren().add(btn);
        residentPane.add(hbBtn, 1, 4);
        modalStage.setScene(modalScene);
        modalStage.showAndWait();

        return resident;
    }
}
