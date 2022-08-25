import java.sql.SQLException;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;
import javafx.scene.layout.HBox;

public class Staff {
    public String name;
    private int id;
    private String phone;
    private String role;
    public HBox hBox;
    public TextField phoneTextField;
    public TextField passwordTextField;
    public Button updatePhone;
    public Button updatePassword;

    public Staff(String name, int id, String role, String phone){
        this.name = name;
        this.id = id;
        this.phone = phone;
        this.role = role;

        hBox = new HBox(8);
        phoneTextField = new TextField(this.phone);
        phoneTextField.setMaxWidth(85);
        updatePhone = new Button("Update");

        updatePhone.setOnAction(new EventHandler<ActionEvent>(){
            @Override
            public void handle(ActionEvent e){
                String newPhone = phoneTextField.getText();
                setPhone(newPhone);
                phoneTextField.setText(newPhone);
                try {
                    DatabaseManager.updateStaffPhone(newPhone, id);
                    DatabaseManager.logEvent(App.session.getStaffID(), "Changed phone number");
                } catch (SQLException e1) {
                    // TODO Auto-generated catch block
                    e1.printStackTrace();
                }
            }
        });

        passwordTextField = new TextField();
        passwordTextField.setMaxWidth(100);
        updatePassword = new Button("Update");

        updatePassword.setOnAction(new EventHandler<ActionEvent>(){
            @Override
            public void handle(ActionEvent e){
                String newPassword = passwordTextField.getText();
                passwordTextField.clear();
                try {
                    DatabaseManager.updateStaffPassword(newPassword, id);
                    DatabaseManager.logEvent(App.session.getStaffID(), "Changed password");
                } catch (SQLException e1) {
                    // TODO Auto-generated catch block
                    e1.printStackTrace();
                }
            }
        });


        hBox.getChildren().addAll(new Label(name), phoneTextField, updatePhone, new Label("Update Pasword: "), passwordTextField, updatePassword);
    }

    public String getRole(){
        return role;
    }

    public void setPhone(String phone){
        this.phone = phone;
    }
}
