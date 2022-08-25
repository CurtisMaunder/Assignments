import java.sql.SQLException;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.TextArea;
import javafx.scene.control.TextField;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.StackPane;
import javafx.scene.paint.Color;
import javafx.scene.shape.Rectangle;
import javafx.stage.Stage;

public class Bed {
    private int bedNum;
    private Resident resident;
    private Rectangle bedRect;
    private Session session;
    static final int BED_RECT_SIZE = 100;

    public Bed(int _bedNum, Resident _resident, Session _session){
        this.bedNum = _bedNum;
        this.session = _session;
        
        bedRect = new Rectangle(BED_RECT_SIZE, BED_RECT_SIZE);
        this.resident = _resident;

        updateBed(resident);

        //Adding event handlers inside the bed object itself, this allows for dynamic generation of objects and gives all control to the class
        bedRect.setOnMouseClicked(new EventHandler<MouseEvent>(){
            @Override
            public void handle(MouseEvent t){
                //Creating modal window if logged in as a manager
                if(session.getRole().equals("M") && resident.getID() == 0){
                    try {
                        addNewResident(Administration.modal(), session);
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }
                }else if(session.getRole().equals("N") && resident.getID() == 0){
                    try {
                        updateBedAndDB(Administration.moveResident(session), session);
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }
                }else if(resident.getID() != 0){
                    //For assigning resident to vacant bed or for viewing resident information
                    Stage modalStage = new Stage();
                    //Closing the stage when it isnt focused, to prevent vacant beds being reoccupied or many windows building up over a session
                    modalStage.focusedProperty().addListener((ov, onHidden, onShown) -> {
                        if(!modalStage.isFocused())
                        modalStage.close();
                    });
                    GridPane loginPane = new GridPane();
                    Label fNameLabel = new Label("First Name:" + resident.getFirstName());
                    Label lNameLabel = new Label("First Name:" + resident.getLastName());
                    int gridIndex = 0;
                    loginPane.add(fNameLabel, 0, gridIndex++);
                    loginPane.add(lNameLabel, 0, gridIndex++);
                    Label prescriptionsTitle = new Label("Prescriptions:" );
                    loginPane.add(prescriptionsTitle, 0, gridIndex++);
                    if(resident.getPrescriptions() != null){
                        for (Prescription p : resident.getPrescriptions()) {
                            Label label = new Label(p.datePrescribed + " " + p.dose + " " + p.interval + " " + p.notes);
                            loginPane.add(label, 0, gridIndex++);
                            gridIndex++;
                        }
                    }

                    if(session.getRole().equals("D")){
                        //new prescription block
                        Label addPrescriptionLabel = new Label("Add Prescription: ");
                        loginPane.add(addPrescriptionLabel, 0, gridIndex++);

                        loginPane.add(new Label("Select Medicine: "), 0, gridIndex++);

                        ComboBox<Medicine> medicineComboBox = new ComboBox<Medicine>();
                        for (Medicine m : App.careHome.medicines) {
                            medicineComboBox.getItems().add(m);
                        }
                        loginPane.add(medicineComboBox, 0, gridIndex++);

                        loginPane.add(new Label("Add dose: "), 0, gridIndex++);

                        TextField addDoseTextField = new TextField();
                        loginPane.add(addDoseTextField , 0, gridIndex++);

                        loginPane.add(new Label("Add interval: "), 0, gridIndex++);

                        TextField addIntervalTextField = new TextField();
                        loginPane.add(addIntervalTextField, 0, gridIndex++);

                        loginPane.add(new Label("Add notes: "), 0, gridIndex++);
                        TextArea addNotesTextArea = new TextArea();
                        loginPane.add(addNotesTextArea, 0, gridIndex++);

                        Button addPrescriptionBtn = new Button("Add Prescription");
                        addPrescriptionBtn.setOnAction(new EventHandler<ActionEvent>(){
                            @Override
                            public void handle(ActionEvent e){
                                //Get current date
                                LocalDateTime date = LocalDateTime.now();
                                DateTimeFormatter dateFormat = DateTimeFormatter.ofPattern("dd/MM/YYYY");
                                String datePrescribed = date.format(dateFormat);
                                                       
                                Medicine tmpMedicine = medicineComboBox.getSelectionModel().getSelectedItem();
                                String dose = addDoseTextField.getText();
                                String interval = addIntervalTextField.getText();
                                String notes = addNotesTextArea.getText();
                                int prescriptionID;
                                try {
                                    prescriptionID = DatabaseManager.addPrescription(tmpMedicine.id, resident.getID(), dose, interval, datePrescribed, notes);
                                    Prescription tmpPrescription = new Prescription(prescriptionID, tmpMedicine.id, resident.getID(), dose, interval, datePrescribed, notes);
                                    resident.addPrescription(tmpPrescription);
                                    DatabaseManager.logEvent(App.session.getStaffID(), resident.getID(), "Added prescription");
                                } catch (SQLException e1) {
                                    e1.printStackTrace();
                                }
                            }
                        });

                        loginPane.add(addPrescriptionBtn, 0, gridIndex++);
                        }

                    Scene loginScene = new Scene(new StackPane(loginPane), 400, 600);
                    modalStage.setScene(loginScene);
                    modalStage.show();
                }
            }
        });
    }

    public void addNewResident(Resident resident, Session session) throws SQLException{
        resident.setBedID(bedNum);
        if(resident.getFirstName().isEmpty() || resident.getLastName().isEmpty() || resident.getFirstName().equals("0") || resident.getLastName().equals("0")){
            return;
        }else{
            DatabaseManager.newResidentToDatabase(resident, session, bedNum);
            updateBed(resident);
        }
        DatabaseManager.logEvent(App.session.getStaffID(), resident.getID(), "Added new resident");
    }

    public void updateBed(Resident resident){
        this.resident = resident;
        //Setting bed colour dependent on vacancy and resident sex
        if(this.resident.getID() == 0)
            bedRect.setFill(Color.WHITE);
        else if(this.resident.getSex().equals("Female"))
            bedRect.setFill(Color.RED);
        else if(this.resident.getSex().equals("Male"))
            bedRect.setFill(Color.BLUE);
    }

    public void updateBedAndDB(Resident resident, Session session) throws SQLException{
        DatabaseManager.updateResident(resident, session, this.bedNum);
        updateBed(resident);
        DatabaseManager.logEvent(App.session.getStaffID(), resident.getID(), "Moved resident to new bed");
    }

    public Rectangle getBedRect(){
        return bedRect;
    }

    public Resident getResident(){
        return resident;
    }

    public void setResident(Resident resident){
        this.resident = resident;
    }
}
