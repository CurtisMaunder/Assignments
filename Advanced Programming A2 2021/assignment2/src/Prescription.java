public class Prescription {
    int id;
    int medicineID;
    int patientID;
    String dose;
    String interval;
    String datePrescribed;
    String notes;

    public Prescription(int id, int medicineID, int patientID, String dose, String interval, String datePrescribed, String notes){
        this.id = id;
        this.medicineID = medicineID;
        this.patientID = patientID;
        this.dose = dose;
        this.interval = interval;
        this.datePrescribed = datePrescribed;
        this.notes = notes;
    }
}
