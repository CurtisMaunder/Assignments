public class Medicine {
    int id;
    String name;

    public Medicine(int id, String name){
        this.id = id;
        this.name = name;
    }

    @Override
    public String toString(){
        return this.name;
    }
}
