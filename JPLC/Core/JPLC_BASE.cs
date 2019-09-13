using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Snap7;

namespace JPLC
{
    public abstract class JPLC_BASE
    {

        #region [Public Propertes]
        public int SizeInBytes { get; internal set; }
        public IEnumerable<PropertyInfo> OrderedProperties {get; private set;}
        public int Address { get; private set; }
        //public Timer Watcher = new Timer();
        public int DBNumber;

        public string LastError { get; private set; } = "";
       // public int WatcherDBNumber;
        #endregion

        #region [Constructor]
        public JPLC_BASE(int address = 0, int dbnumber = 0)
        {
            DBNumber = dbnumber;
            Address = address;
            CreateOrderedProperties();
        }
        #endregion

        #region [Methods]
        public void ReadFromByteArray(byte[] data)
        {
            var walker = new JPLC_BASEWalker(this, OrderedProperties);

            //===================================================================
            // BOOL 
            //===================================================================
            walker.BooleanFound += (propertyWrapper, offsetFromStartOfWalk) => {
                JPLCProperty<bool> JPLCProperty = propertyWrapper.Property as JPLCProperty<bool>;
               // int bitNumber = (int)((offsetFromStartOfWalk - (int)offsetFromStartOfWalk)*10); // (2.3 -2)*10 = 3
               // int bitNumber = (int)((offsetFromStartOfWalk * 1.0 - ((int)offsetFromStartOfWalk * 1.0)) * 10.0); // (2.3 -2)*10 = 3
                double truncatedOffset = Math.Truncate(offsetFromStartOfWalk);
                double difference = Math.Round((offsetFromStartOfWalk - truncatedOffset) * 10);
                int bitNumber = (int)(difference); // (2.3 -2)*10 = 3
                JPLCProperty.Value = S7.GetBitAt(data, (int)offsetFromStartOfWalk, bitNumber);
            };

            //===================================================================
            // BYTE 
            //===================================================================
            walker.ByteFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<byte> JPLCProperty = propertyWrapper.Property as JPLCProperty<byte>;
                JPLCProperty.Value = S7.GetByteAt(data, (int)offsetFromStartOfWalk);
            };

            //===================================================================
            // INT 
            //===================================================================
            walker.IntegerFound += (propertyWrapper, offsetFromStartOfWalk) => {
                JPLCProperty<int> JPLCProperty = propertyWrapper.Property as JPLCProperty<int>;
                JPLCProperty.Value = S7.GetDIntAt(data, (int)offsetFromStartOfWalk);
            };

            //===================================================================
            // REAL 
            //===================================================================
            walker.RealFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<float> JPLCProperty = propertyWrapper.Property as JPLCProperty<float>;
                JPLCProperty.Value = S7.GetRealAt(data, (int)offsetFromStartOfWalk);
            };

            //===================================================================
            // SHORT 
            //===================================================================
            walker.ShortFound += (propertyWrapper, offsetFromStartOfWalk) => {
                JPLCProperty<short> JPLCProperty = propertyWrapper.Property as JPLCProperty<short>;
                JPLCProperty.Value = (short)S7.GetIntAt(data, (int)offsetFromStartOfWalk);
            };

            //===================================================================
            // DATETIME 
            //===================================================================
            walker.DateTimeFound += (propertyWrapper, offsetFromStartOfWalk) => {
                JPLCProperty<DateTime> JPLCProperty = propertyWrapper.Property as JPLCProperty<DateTime>;
                JPLCProperty.Value = S7.GetDateTimeAt(data, (int)offsetFromStartOfWalk);
            };

            //===================================================================
            // DATE 
            //===================================================================
            walker.DateFound += (propertyWrapper, offsetFromStartOfWalk) => {
                JPLCProperty<S7Date> JPLCProperty = propertyWrapper.Property as JPLCProperty<S7Date>;
                JPLCProperty.Value = new S7Date();
                JPLCProperty.Value.Date = S7.GetDateAt(data, (int)offsetFromStartOfWalk);
            };

            //===================================================================
            // TIME 
            //===================================================================
            walker.TimeFound += (propertyWrapper, offsetFromStartOfWalk) => {
                JPLCProperty<S7Time> JPLCProperty = propertyWrapper.Property as JPLCProperty<S7Time>;
                JPLCProperty.Value = new S7Time();
                JPLCProperty.Value.Time = S7.GetTODAt(data, (int)offsetFromStartOfWalk);

            };

            //===================================================================
            // UDT 
            //===================================================================
            walker.UDTFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
              //  Console.WriteLine(propertyWrapper.Property);
                var udt = (propertyWrapper.PropertyType.GetProperty("Value").GetValue(propertyWrapper.Property, null)) as JPLC_BASE;
                byte[] extractedByteArray = data.Skip((int)offsetFromStartOfWalk).Take(udt.SizeInBytes).ToArray();
                udt.ReadFromByteArray(extractedByteArray);
            };

            //===================================================================
            // STRING 
            //===================================================================
            walker.StringFound += (propertyWrapper, offsetFromStartOfWalk) => {
                JPLCProperty<string> JPLCProperty = propertyWrapper.Property as JPLCProperty<string>;
                JPLCProperty.Value = S7.GetStringAt(data, (int)offsetFromStartOfWalk);
            };

            walker.Walk();


        }
        public byte[] WriteToByteArray()
        {
            byte[] data = new byte[SizeInBytes];
            var walker = new JPLC_BASEWalker(this, OrderedProperties);

            //===================================================================
            // BOOL 
            //===================================================================
            walker.BooleanFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<bool> JPLCProperty = propertyWrapper.Property as JPLCProperty<bool>;
                double truncatedOffset = Math.Truncate(offsetFromStartOfWalk);
                double difference = Math.Round((offsetFromStartOfWalk - truncatedOffset)*10);
                int bitNumber = (int)(difference); // (2.3 -2)*10 = 3

                S7.SetBitAt(ref data, (int)(offsetFromStartOfWalk), bitNumber, JPLCProperty.Value);
            };

            //===================================================================
            // INT 
            //===================================================================
            walker.IntegerFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<int> JPLCProperty = propertyWrapper.Property as JPLCProperty<int>;
                S7.SetDIntAt(data, (int)offsetFromStartOfWalk, JPLCProperty.Value);
            };

            //===================================================================
            // REAL 
            //===================================================================
            walker.RealFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<float> JPLCProperty = propertyWrapper.Property as JPLCProperty<float>;
                S7.SetRealAt(data, (int)offsetFromStartOfWalk, JPLCProperty.Value);
            };

            //===================================================================
            // SHORT 
            //===================================================================
            walker.ShortFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<short> JPLCProperty = propertyWrapper.Property as JPLCProperty<short>;
                S7.SetIntAt(data, (int)offsetFromStartOfWalk, JPLCProperty.Value);
            };

            //===================================================================
            // BYTE 
            //===================================================================
            walker.ByteFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<byte> JPLCProperty = propertyWrapper.Property as JPLCProperty<byte>;
                S7.SetByteAt(data, (int)offsetFromStartOfWalk, JPLCProperty.Value);
            };


            //===================================================================
            // DATETIME 
            //===================================================================
            walker.DateTimeFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<DateTime> JPLCProperty = propertyWrapper.Property as JPLCProperty<DateTime>;
                S7.SetDateTimeAt(data, (int)offsetFromStartOfWalk, JPLCProperty.Value);
            };

            //===================================================================
            // DATE 
            //===================================================================
            walker.DateFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<S7Date> JPLCProperty = propertyWrapper.Property as JPLCProperty<S7Date>;
                S7.SetDateAt(data, (int)offsetFromStartOfWalk, JPLCProperty.Value.Date);


            };

            //===================================================================
            // TIME 
            //===================================================================
            walker.TimeFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<S7Time> JPLCProperty = propertyWrapper.Property as JPLCProperty<S7Time>;
                S7.SetTODAt(data, (int)offsetFromStartOfWalk, JPLCProperty.Value.Time);
            };

            //===================================================================
            // UDT 
            //===================================================================
            walker.UDTFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                var udt = (propertyWrapper.PropertyType.GetProperty("Value").GetValue(propertyWrapper.Property, null)) as JPLC_BASE;
                byte[] udtByteArray = udt.WriteToByteArray();
                // Add byte array to master byte array
                for (int i = 0; i < udtByteArray.Length; i++)
                {
                    data[(int)offsetFromStartOfWalk + i] = udtByteArray[i];
                }
            };

            //===================================================================
            // STRING 
            //===================================================================
            walker.StringFound += (propertyWrapper, offsetFromStartOfWalk) =>
            {
                JPLCProperty<string> JPLCProperty = propertyWrapper.Property as JPLCProperty<string>;
                S7.SetStringAt(data, (int)offsetFromStartOfWalk, propertyWrapper.StringAttribute.PLCString, JPLCProperty.Value);
            };

            walker.Walk();
            return data;
        }



        public int ReadFromDB(JPLCConnection connection, int dbNumber = 0)
        {
            if (!connection.Connected)
            {
                throw new TargetException("JPLC not connected to a PLC");
            }
            if (dbNumber == 0)
            {
                dbNumber = DBNumber;
            }
            byte[] data = new byte[this.SizeInBytes];
            int result = connection.S7Api.DBRead(dbNumber, this.Address, this.SizeInBytes, data);
            
            if (result != 0)
            {
                LastError = connection.S7Api.ErrorText(result);
                return result;
            }
            ReadFromByteArray(data);

            return result;
        }

        public int Read(JPLCConnection connection)
        {
            return ReadFromDB(connection);
        }

        public int WriteToDB(JPLCConnection connection, int dbNumber =0)
        {
            if (!connection.Connected)
            {
                throw new TargetException("JPLC not connected to a PLC");
            }
            if (dbNumber == 0)
            {
                dbNumber = DBNumber;
            }
            byte[] data = WriteToByteArray();
            int result = connection.S7Api.DBWrite(dbNumber, this.Address, this.SizeInBytes, data);
            
            if (result != 0)
            {
                LastError = connection.S7Api.ErrorText(result);

                return result;
            }
            return result;
 
        }

        public int Write(JPLCConnection connection)
        {
            return WriteToDB(connection);
        }
        #endregion

        #region [Private Methods]
        private void CreateOrderedProperties()
        {
            OrderedProperties=   from property in this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
                                 let orderAttribute = (property.GetCustomAttributes(typeof(OrderAttribute), false).SingleOrDefault() as OrderAttribute)
                                 orderby orderAttribute.Order
                                 select property;
            InitializeProperties();
        }

        private void InitializeProperties()
        {
            /*
             * Walk through the properties in this class
             * eg
             * Class UDT1 { 
             * JPLCProperty<bool> a; 
             * JPLCProperty<int> b; 
             * JPLCProperty<short> c; 
             * }
             */
            var walker = new JPLC_BASEWalker(this, OrderedProperties);
            //===================================================================
            // BOOL 
            //===================================================================
            walker.BooleanFound += (propertyWrapper, offsetFromStartOfWalk) => { propertyWrapper.Property = new JPLCProperty<bool>(propertyWrapper.Name, this, offsetFromStartOfWalk); };
            
            //===================================================================
            // INT 
            //===================================================================
            walker.IntegerFound += (propertyWrapper, offsetFromStartOfWalk) => { propertyWrapper.Property = new JPLCProperty<int>(propertyWrapper.Name, this, offsetFromStartOfWalk); };

            //===================================================================
            // REAL 
            //===================================================================
            walker.RealFound += (propertyWrapper, offsetFromStartOfWalk) => { propertyWrapper.Property = new JPLCProperty<float>(propertyWrapper.Name, this, offsetFromStartOfWalk); };     
      
            //===================================================================
            // SHORT 
            //===================================================================
            walker.ShortFound += (propertyWrapper, offsetFromStartOfWalk) => { propertyWrapper.Property = new JPLCProperty<short>(propertyWrapper.Name, this, offsetFromStartOfWalk); };

            //===================================================================
            // BYTE 
            //===================================================================
            walker.ByteFound += (propertyWrapper, offsetFromStartOfWalk) => { propertyWrapper.Property = new JPLCProperty<byte>(propertyWrapper.Name, this, offsetFromStartOfWalk); };

            //===================================================================
            // DATETIME 
            //===================================================================
            walker.DateTimeFound += (propertyWrapper, offsetFromStartOfWalk) => { propertyWrapper.Property = new JPLCProperty<DateTime>(propertyWrapper.Name, this, offsetFromStartOfWalk); };

            //===================================================================
            // DATE 
            //===================================================================
            walker.DateFound += (propertyWrapper, offsetFromStartOfWalk) => { 
                JPLCProperty<S7Date> date = new JPLCProperty<S7Date>(propertyWrapper.Name, this, offsetFromStartOfWalk); date.Value = new S7Date(); propertyWrapper.Property = date; };

            //===================================================================
            // TIME 
            //===================================================================
            walker.TimeFound += (propertyWrapper, offsetFromStartOfWalk) => { JPLCProperty<S7Time> time = new JPLCProperty<S7Time>(propertyWrapper.Name, this, offsetFromStartOfWalk); time.Value = new S7Time(); propertyWrapper.Property = time; };

            //===================================================================
            // UDT 
            //===================================================================
            walker.UDTFound += (propertyWrapper, offsetFromStartOfWalk) => {
                Type genericType = propertyWrapper.PropertyType.GetGenericArguments()[0];
                var udt = Activator.CreateInstance(genericType, (int)(offsetFromStartOfWalk + this.Address)) as JPLC_BASE; // Creates the udt
                propertyWrapper.Property = Activator.CreateInstance(propertyWrapper.PropertyType, udt, propertyWrapper.Name, this, (int)offsetFromStartOfWalk); // Creates the PLCProperty<udt>
            };
            
            //===================================================================
            // STRING 
            //===================================================================
            walker.StringFound += (propertyWrapper, offsetFromStartOfWalk) => {
                var stringProperty = new JPLCProperty<string>(propertyWrapper.Name, this, offsetFromStartOfWalk);
                stringProperty.Value = "";
                propertyWrapper.Property = stringProperty;   
            };

            //===================================================================
            // END OF WALK 
            //===================================================================
            walker.WalkCompleted += (offsetFromStartOfWalk) => { SizeInBytes = (int)offsetFromStartOfWalk; };
            walker.Walk();

        }

        #endregion
    }
    public delegate void BooleanFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void ShortFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void IntegerFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void ByteFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void StringFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void DateTimeFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void TimeFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void DateFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void RealFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);
    public delegate void UDTFoundHandler(JPLCPropertyInfo prop, double offsetFromStartOfWalk);

    public delegate void WalkCompletedHandler(double offsetFromStartOfWalk);

    class JPLC_BASEWalker
    {
        #region [Private]
        private object _ObjectToWalk;
        private IEnumerable<PropertyInfo> _OrderedProperties;
        #endregion

        #region [Events]
        public event BooleanFoundHandler BooleanFound = delegate { };
        public event ShortFoundHandler ShortFound = delegate { };
        public event IntegerFoundHandler IntegerFound = delegate { };
        public event ByteFoundHandler ByteFound = delegate { };
        public event StringFoundHandler StringFound = delegate { };
        public event DateTimeFoundHandler DateTimeFound = delegate { };
        public event DateFoundHandler DateFound = delegate { };
        public event TimeFoundHandler TimeFound = delegate { };
        public event RealFoundHandler RealFound = delegate { };
        public event UDTFoundHandler UDTFound = delegate { };
        public event WalkCompletedHandler WalkCompleted = delegate { };
        #endregion


        #region [Constructor]
        public JPLC_BASEWalker(object objectToWalk, IEnumerable<PropertyInfo> orderedProperties)
        {
            _ObjectToWalk = objectToWalk;
            _OrderedProperties = orderedProperties;
        }
        #endregion

        #region [Methods]
        public void Walk()
        {
            int p = 0; // Offset from start of walk
            bool lastElementWasBit = false; // Last element walked was a bit
            int lastElementBitNum = 0; // Last bit number that was walked

            // We are going to dig through every property in the class in the order they were listed and raise events according to what we see
            foreach (var orderedProperty in _OrderedProperties)
            {
                // We will treat every propert as though it were an array and put all the elements inside that array inside this list. If the propery is not an array, this list will have one element inside it.
                var arrayPropertyList = new List<JPLCPropertyInfo>();
                // This grabs the actual value of this property as stored in the instance _ObjectToWalk
                var currentOrderedProperty = orderedProperty.GetValue(_ObjectToWalk, null);

                PLCStringAttribute stringAttribute = null;

                // Check if element is an array and disassemble it to individual properties to go through individually later
                // The reason we are doing this is because we are dealing with properties found through reflection. With an array, reflection will only tell us 
                // we have a property of array type. Size and member elements are evaluated at runtime. What we want is to treat each member of the array as an individual 
                // property rather than deal with the whole array as one property. Because of this, we must take the array property and make multiple properties representing the 
                // the elements it has to hold

                int rank = 0;
                if (orderedProperty.PropertyType.IsArray)
                {
                    // This will show us what the array is actually holding
                    var arrayElementType = orderedProperty.PropertyType.GetElementType();
                    // This will make sure the array hold JPLCPropery<> only, anything else should throw an error
                    if (!(arrayElementType.IsGenericType && arrayElementType.GetGenericTypeDefinition() == typeof(JPLCProperty<>)))
                    {
                        throw new NotSupportedException("You can only have JPLCProperty<> in your PLC classes");
                    }
                    if (arrayElementType == typeof(JPLCProperty<string>))
                    {
                        stringAttribute = (orderedProperty.GetCustomAttributes(typeof(PLCStringAttribute), false).SingleOrDefault() as PLCStringAttribute);
                    }
                    

                    // We have to figure out if this is a single array or a double array
                    rank = orderedProperty.PropertyType.GetArrayRank();
                    if (rank == 1) // If we have a single dimension array
                    {
                        // We expect an attribute above the array telling us how many members this array contains
                        ArraySizeAttribute arraySizeAttribute = (orderedProperty.GetCustomAttributes(typeof(ArraySizeAttribute), false).SingleOrDefault() as ArraySizeAttribute);
                        Array array = currentOrderedProperty as Array; // Current property casted to array
                        if (array == null)
                        {
                            array = Activator.CreateInstance(orderedProperty.PropertyType, new object[] { arraySizeAttribute.ArraySize }) as Array;
                            currentOrderedProperty = array;
                        }
                        //  Loop through all the members and create new JPLCPropertyInfo for each which holds all the information we need
                        for (int i = 0; i < arraySizeAttribute.ArraySize; i++) 
                        {
                            var prop = new JPLCPropertyInfo(orderedProperty.Name + i, array.GetValue(i), arrayElementType);
                            prop.StringAttribute = stringAttribute;
                            arrayPropertyList.Add(prop); // Add it to the list
                        }

                    }
                    else if (rank == 2) // If we have a double dimension array
                    {
                        DoubleArraySizeAttribute doubleArraySizeAttribute = (orderedProperty.GetCustomAttributes(typeof(DoubleArraySizeAttribute), false).SingleOrDefault() as DoubleArraySizeAttribute);
                        Array array = currentOrderedProperty as Array; // Current property casted to array

                        if (array == null)
                        {
                            array = Activator.CreateInstance(orderedProperty.PropertyType, new object[] { doubleArraySizeAttribute.Array1Size, doubleArraySizeAttribute.Array2Size }) as Array;
                            currentOrderedProperty = array;
                        }
                        for (int i = 0; i < doubleArraySizeAttribute.Array1Size; i++)
                        {
                            for (int j = 0; j < doubleArraySizeAttribute.Array2Size; j++)
                            {
                                var prop = new JPLCPropertyInfo(orderedProperty.Name + i+"_"+j, array.GetValue(i,j), arrayElementType);
                                prop.StringAttribute = stringAttribute;
                                arrayPropertyList.Add(prop); // Add it to the list
                            }
                        }

                    }
                    else
                    {
                        throw new NotSupportedException("We only support single or double arrays");
                    }
                }
                // If the property is not an array, add only one property info to the property list
                else
                {
                    if (orderedProperty.PropertyType == typeof(JPLCProperty<string>))
                    {
                        stringAttribute = (orderedProperty.GetCustomAttributes(typeof(PLCStringAttribute), false).SingleOrDefault() as PLCStringAttribute);
                    }
                    var prop = new JPLCPropertyInfo(orderedProperty.Name, currentOrderedProperty, orderedProperty.PropertyType);
                    prop.StringAttribute = stringAttribute;
                    arrayPropertyList.Add(prop);

                }
                int counter = 0;
                // Loop through the list we made up there. Remember, if the variable is not an array ->  the arrayPropertyList will only hold one variable 
                foreach (var property in arrayPropertyList)
                {
                    
                    var currentproperty = property.Property;
                    //===================================================================
                    // SHORT 
                    //===================================================================
                    if (property.PropertyType == typeof(JPLCProperty<short>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); } // This makes sure that we always start a short at an even address
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        JPLCProperty<short> JPLCProperty = currentproperty as JPLCProperty<short>;
                        ShortFound(property, p); // Raise Event
                        
                        p += sizeof(short); // Advance Offset Pointer
                    }
                    //===================================================================
                    // BOOL 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<bool>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (!lastElementWasBit) { lastElementBitNum = 0; }
                        if (lastElementBitNum > 7) { p += sizeof(byte); lastElementBitNum = 0; }
                        // --------------------------------------------------------------



                        BooleanFound(property, p + 0.1 * lastElementBitNum); // Raise Event
                        lastElementBitNum++; // Advance Bit Pointer
                        lastElementWasBit = true; // Set last element was bit flag
                    }
                    //===================================================================
                    // INT 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<int>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); }
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        IntegerFound(property, p); // Raise Event
                        p += sizeof(int); // Advance Offset Pointer
                    }
                    //===================================================================
                    // BYTE 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<byte>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (lastElementWasBit) { p += sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        ByteFound(property, p); // Raise Event
                        p += sizeof(byte); // Advance Offset Pointer

                    }
                    //===================================================================
                    // STRING 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<string>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); }
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        StringFound(property, p); // Raise Event
                        p += property.StringAttribute.PLCString + 2; // Advance Offset Pointer                  
                    }
                    //===================================================================
                    // REAL 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<float>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); }
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        RealFound(property, p); // Raise Event
                        p += 4; // Advance Offset Pointer                  
                    }
                    //===================================================================
                    // DATETIME 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<DateTime>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); }
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        DateTimeFound(property, p); // Raise Event
                        p += 8; // Advance Offset Pointer                  
                    }
                    //===================================================================
                    // DATE 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<S7Date>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); }
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        DateFound(property, p); // Raise Event
                        p += 3; // Advance Offset Pointer                  
                    }
                    //===================================================================
                    // TIME 
                    //===================================================================
                    else if (property.PropertyType == typeof(JPLCProperty<S7Time>))
                    {
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); }
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------

                        TimeFound(property, p); // Raise Event
                        p += 3; // Advance Offset Pointer                  
                    }
                    //===================================================================
                    // UDT 
                    //===================================================================
                    else if (property.PropertyType.GetGenericTypeDefinition() == typeof(JPLCProperty<>)) // eg JPLCProperty<string>
                    {
                        Type genericType = property.PropertyType.GetGenericArguments()[0]; //eg. String
                        if (!genericType.IsSubclassOf(typeof(JPLC_BASE)))
                        {
                            throw new Exception("Your PLC classes have types that are not supported");
                        }
                        // Advance Offset Pointer ---------------------------------------
                        if (p % 2 != 0) { p += sizeof(byte); }
                        if (lastElementWasBit) { p += 2 * sizeof(byte); lastElementWasBit = false; }
                        // --------------------------------------------------------------
                        UDTFound(property, p); // Raise Event
                        currentproperty = property.Property;
                        var udt = (property.PropertyType.GetProperty("Value").GetValue(currentproperty,null)) as JPLC_BASE;
                        if (udt == null)
                        {
                            throw new NullReferenceException("UDT has not been initialized");
                        }
                        p += udt.SizeInBytes; // Advance Offset Pointer  


                    }
                    if (!orderedProperty.PropertyType.IsArray)
                    {
                        orderedProperty.SetValue(_ObjectToWalk, property.Property, null);
                        
                    }
                    else if (orderedProperty.PropertyType.IsArray && rank == 1)
                    {
                        
                        Array array = currentOrderedProperty as Array; // Current property casted to array
                        array.SetValue(property.Property, counter);
                    }
                    else if (orderedProperty.PropertyType.IsArray && rank == 2)
                    {

                        Array array = currentOrderedProperty as Array; // Current property casted to array
                        array.SetValue(property.Property, counter / array.GetLength(1), counter % array.GetLength(1));
                    }
                    counter++;
                }
                if (orderedProperty.PropertyType.IsArray){
                    orderedProperty.SetValue(_ObjectToWalk, currentOrderedProperty, null);
                }
                

                
            }


            // If we end with a boolean, we will offset the pointer to complete the walk
            if (lastElementWasBit)
            {
                if (p % 2 == 0)
                {
                    p += 2 * sizeof(byte); lastElementWasBit = false;
                }
                else
                {
                    p += sizeof(byte); lastElementWasBit = false;
                }
            }
            WalkCompleted(p);
        }
        #endregion
    }

    public class JPLCPropertyInfo
    {
        #region [Public]
        public string Name { get; set; }
        public object Property { get; set; }
        public Type PropertyType { get; set; }
        public PLCStringAttribute StringAttribute { get; set; } 
        #endregion

        #region [Constructor]
        public JPLCPropertyInfo(string name, object property, Type propertyType)
        {
            Name = name;
            Property = property;
            PropertyType = propertyType;
        }
        #endregion

    }

    public class S7Date
    {
        public DateTime Date = new DateTime();
    }
    public class S7Time
    {
        public DateTime Time = new DateTime();
    }
}
