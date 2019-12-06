# Notice
[S7NetPlus](https://github.com/S7NetPlus) is a similar project that does what this project initially aimed to do. This project differs in that it allows you to write and read arrays and strings of various sizes. It is also possible to read and write an individual property rather than a whole DB.

# JPLC
This is code that augments [Snap7](http://snap7.sourceforge.net/) functionality so that you don't have to deal with bytes directly and can transfer information from a Siemens PLC directly to C# in a seamless manner. It allows you to describe UDTs and datablocks in C# using attributes. 

## Use
Lets say we had a datablock in a Siemens PLC called MyDatablock with various types of properties inside it. 

To define it in JPLC, we make a class which inherits from JPLC_BASE. 


```c#
using JPLC;
public class MyUDT : JPLC_BASE
{
    [Order(1)]
    public JPLCProperty<int> MyInteger { get; set; }
    
    public MyUDT(int address = 0) : base(address) { }
}
    
public class MyDatablock : JPLC_BASE
{
    [Order(1)]
    public JPLCProperty<int> MyInteger { get; set; }
    [Order(2)]
    public JPLCProperty<bool> MyBool { get; set; }
    [Order(3)]
    public JPLCProperty<MyUDT> MyCustomUDT { get; set; }
    [Order(4)]
    public JPLCProperty<DateTime> MyDateTime { get; set; }
    [Order(5)]
    public JPLCProperty<short> MyShort { get; set; }
    [Order(6)]
    public JPLCProperty<byte> MyByte { get; set; }
    [Order(7)]
    [ArraySize(10)]
    public JPLCProperty<int>[] SimpleArray { get; set; }
    [Order(8)]
    [ArraySize(10)]
    public JPLCProperty<MyUDT>[] UDTArray { get; set; }
    [Order(9)]
    [PLCString(150)]
    public JPLCProperty<string> MyString { get; set; }
    [Order(10)]
    public JPLCProperty<float> MyReal { get; set; }
    
    public MyDatablock(int address = 0) : base(address) { }
}
```

## Connecting to the PLC

JPLCConnection is a wrapper around the S7Client.

```c#
var connection = JPLCConnection("192.168.0.1", 0 ,2)
connection.Connect(); 

var myDatablock = new MyDatablock();
myDatablock.ReadFromDB(connection, 24); // 24: Datablock Number

// To Write
myDatablock.MyBool.Value = false;
myDatablock.MyInteger.Value = 3;
myDatablock.Write();
// OR
myDatablock.WriteToDB(24);
```

## Restrictions
Must be non-optimized datablock.

