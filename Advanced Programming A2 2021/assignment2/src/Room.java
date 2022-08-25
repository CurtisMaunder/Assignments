import java.util.ArrayList;

import javafx.geometry.Insets;
import javafx.scene.control.Label;
import javafx.scene.layout.GridPane;

public class Room {
    private int roomNum;
    public ArrayList<Bed> beds;
    public GridPane roomPane;
    private int[] gridPosition = {1, 0};

    public Room(int roomNum){
        this.roomNum = roomNum;
        beds = new ArrayList<Bed>();
        roomPane = new GridPane();
        roomPane.setHgap(10);
        roomPane.setVgap(10);
        roomPane.setStyle("-fx-border-color: black");
        roomPane.setPadding(new Insets(10));
        roomPane.add(new Label("Room " + roomNum), 0, 0);
    }

    public void addBed(Bed bed){
        beds.add(bed);
        roomPane.add(bed.getBedRect(), gridPosition[1], gridPosition[0]);

        if(gridPosition[1] == 1){
            gridPosition[0]++;
            gridPosition[1] = 0;
        }else if(gridPosition[1] == 0){
            gridPosition[1]++;
        }
    }
}
