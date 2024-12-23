using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper
{
    public class PropToString
    {
        public static void PrintData<T>(T obj)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                PropertyInfo[] properties = type.GetProperties();
                Console.WriteLine($"Propiedades de {type.Name}:");
                foreach (PropertyInfo property in properties)
                {
                    object? value = property.GetValue(obj);
                    if (value != null)
                    {
                        if (value is Array arrayValue)
                        {
                            Console.WriteLine($"{property.Name}:");
                            foreach (object item in arrayValue)
                            {
                                PrintData(item);
                            }
                        }
                        else if (value is IEnumerable enumerableValue && !(value is string))
                        {
                            Console.WriteLine($"{property.Name}:");
                            foreach (object item in enumerableValue)
                            {
                                PrintData(item);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{property.Name}: {value}");
                        }
                    }
                }
            }

        }

        public static void PrintListData<T>(IEnumerable<T> list)
        {
            if (list == null)
            {
                Console.WriteLine("List is null");
                return;
            }

            int index = 0;
            foreach (T item in list)
            {
                Console.WriteLine($"Elemento {index++}:");
                PrintData(item);
                Console.WriteLine();
            }
        }


    }
}