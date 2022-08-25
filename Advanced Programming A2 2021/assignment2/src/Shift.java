public class Shift {
    private int id;
    private String day;
    private String timeSlot;
    private int staffID;

    public Shift(int id, String day, String timeSlot, int staffID){
        this.id = id;
        this.day = day;
        this.timeSlot = timeSlot;
        this.staffID = staffID;
    }
}
