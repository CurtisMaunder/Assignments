import java.sql.Connection;
import java.sql.SQLException;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Pos;
import javafx.geometry.Insets;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.PasswordField;
import javafx.scene.control.TextField;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.HBox;
import javafx.scene.layout.StackPane;
import javafx.scene.paint.Color;
import javafx.scene.text.Text;
import javafx.stage.Stage;
import javafx.stage.WindowEvent;

public class Login {
    static Session login(Connection conn, Stage stage){
        Session session = new Session("", 0, conn);
        Stage loginStage = new Stage();
        GridPane loginPane = new GridPane();
        loginPane.setAlignment(Pos.CENTER);
        loginPane.setHgap(10);
        loginPane.setVgap(10);
        loginPane.setPadding(new javafx.geometry.Insets(25));
        Scene loginScene = new Scene(new StackPane(loginPane), 400, 600);

        Label userName = new Label("Staff ID");
        loginPane.add(userName, 0, 1);

        TextField userTextField = new TextField();
        loginPane.add(userTextField, 1, 1);

        Label passLabel = new Label("Password");
        loginPane.add(passLabel, 0, 2);

        PasswordField passwordField = new PasswordField();
        loginPane.add(passwordField, 1, 2);

        Button btn = new Button("Sign in");
        HBox hbBtn = new HBox(10);
        hbBtn.setAlignment(Pos.BOTTOM_RIGHT);
        hbBtn.getChildren().add(btn);
        loginPane.add(hbBtn, 1, 4);

        final Text actiontarget = new Text();
        loginPane.add(actiontarget, 1, 6);

        btn.setOnAction(new EventHandler<ActionEvent>(){
            @Override
            public void handle(ActionEvent e){
                try {
                    if(DatabaseManager.validateLogin(userTextField.getText(), passwordField.getText(), conn)){
                        session.setRole(DatabaseManager.getLoginRole(userTextField.getText(), conn));
                        session.setStaffID(Integer.parseInt(userTextField.getText()));
                        loginStage.close();
                    }else{
                        actiontarget.setFill(Color.FIREBRICK);
                        actiontarget.setText("Invalid Staff ID or password");
                    }
                } catch (SQLException e1) {
                    actiontarget.setFill(Color.FIREBRICK);
                    actiontarget.setText("Staff ID not found");
                }
            }
        });

        loginStage.setOnCloseRequest(new EventHandler<WindowEvent>(){
            @Override
            public void handle(WindowEvent t){
                loginStage.close();
                stage.close();
            }
        });
        loginStage.setScene(loginScene);
        loginStage.showAndWait();

        return session;
    }
}
