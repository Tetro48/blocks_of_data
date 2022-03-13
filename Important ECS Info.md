Requirements for a working entity component
-------------------------------------------

An IComponentData implementation must be a struct and can only contain unmanaged, blittable types, including:

* C#-defined blittable types
* bool
* char
* BlobAssetReference<T\> (a reference to a Blob data structure)
* FixedString (a fixed-sized character buffer)
* Unity.Collections.FixedList
* fixed arrays (in an unsafe context)
* structs containing these unmanaged, blittable fields

Further reading at [Unity's documentation on IComponentData](https://docs.unity3d.com/Packages/com.unity.entities@0.16/api/Unity.Entities.IComponentData.html).

Using ints as an example of a simple blittable value.

Examples:
* Blittable values
```csharp
public struct someCustomComponent : IComponentData
{
    //example variables shown here barely scratches the surface of what's allowed.
    public int integer;
    public int2 integer2;
    public int2x2 integer2x2;
    public int4 integer4;
    public int4x2 integer4x2;
    public int4x4 integer4x4;

    //No pre-assignment is permitted
    public int preassigned_int = 333;
}
```
* Unallowed in unmanaged IComponentData:
```csharp
public struct someCustomComponent : IComponentData
{
    //Resizeable structs and/or arrays are not allowed
    public int[] intArray;
    public List<int> intList;
    public Dictionary<key, int> intDictionary;
    public NativeArray<int> intNativeArray;
    public NativeList<int> intNativeList;
    public NativeSlice<int> intNativeSlice;
    public DynamicBuffer<int> intDynamicBuffer;

    //References are not allowed
    public anyClass classObject;
    public object objectReference;
}
```
