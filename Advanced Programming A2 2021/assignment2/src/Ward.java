import java.util.ArrayList;

import javafx.geometry.Insets;
import javafx.scene.control.Label;
import javafx.scene.layout.GridPane;

public class Ward {
    private String name;
    private int id;
    public ArrayList<Room> rooms;
    public GridPane wardPane;
    private int[] gridPosition = {1, 0};

    public Ward(String name, int id){
        this.name = name;
        this.id = id;
        rooms = new ArrayList<Room>();
        wardPane = new GridPane();
        wardPane.setStyle("-fx-border-color: black");
        wardPane.setPadding(new Insets(5));

        wardPane.add(new Label("Ward"), 0, 0);
    }

    public String getName(){
        return name;
    }

    public int getID(){
        return this.id;
    }

    public void addRoom(Room room){
        rooms.add(room);
        wardPane.add(room.roomPane, gridPosition[1], gridPosition[0]);

        if(gridPosition[1] == 1){
            gridPosition[0]++;
            gridPosition[1] = 0;
        }else if(gridPosition[1] == 0){
            gridPosition[1]++;
        }
    }
}
