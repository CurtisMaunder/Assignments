##IDE 
Visual Studio Code was used

##Version details
jdk-11
sqlit-jbdc-3.34
javafx-11

##Installation and running
Extract as is to install

Compile with:
javac --module-path [javafx-11 path] --add-modules javafx.controls *.java

Run with:
java --module-path [javafx-11 path] --add-modules javafx.controls -classpath ";sqlite-jdbc-3.34.0.jar" App.java