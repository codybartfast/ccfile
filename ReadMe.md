CCFile
======

Read, write and serialize to file - simply and robustly.

Features:

* Thread-safe reads and writes (similar to `File.ReadAllText`) plus
  serialization.
* Convenient, thread-safe `ReadOrWrite` method for initialization (similar
  to [`ConcurrentDictionary.GetOrAdd`][MSGetOrAdd]).
* Thread-safe `Modify` method.
* Automatically creates a backup of existing files.
* Supports additional archiving or versioning of files.
* Reduces likelihood of partial writes and detects them if they do happen.

__Warning:__ _Although intended to be robust, this is a first release and so may
not be as reliable as hoped. It may have dreadful bugs._

&nbsp;

For Me, By Me (FMBM)
--------------------

This was created primarily for use by the author.  It has only been tested
in limited environments.  It is intended for getting ad-hoc  applications up
and running quickly.  It probably is not suitable for complex, production,
nor evolving projects.  (The name is inspired by the [Fubu][Fubu],
 _For Us, By Us_, project, but there is no other connection.)

&nbsp;

Contents
--------

[CCFile Basic Usage](#ccfile-basic-usage)  
[CCValue Basic Usage](#ccvalue-basic-usage)  
[Symbolic Links](#symbolic-links)  
[ReadOrWrite](#readorwrite)  
[Modify](#modify)  
[Exists and Delete](#exists-and-delete)  
[Archive](#archive)  
[Interfaces](#interfaces)  
[Files Check](#files-check)  
[Name](#why-is-it-called-ccfile)  

&nbsp;

CCFile Basic Usage
------------------

`CCFile` supports `Read`, `Write`, `Modify`, and `ReadOrWrite` for each of
`bytes[]`, `string` and 'values'.  

This shows a 'value' being written to disk and then read back as a `string`,
as a `byte[]`, and as a 'value':

```csharp
using Fmbm.IO;

// Create new CCFile
var ccfile = new CCFile("CCFile_Sample.txt");

// Serialize a list to disk
ccfile.WriteValue(new List<string> { "Apple", "Banana", "Cherry" });

// Read file contents as text
Console.WriteLine(ccfile.ReadText());

// Read file contents as bytes
Console.WriteLine(ccfile.ReadBytes().Length);

// Read file contents as a value
Console.WriteLine(ccfile.ReadValue<List<string>>().Last());

// OUTPUT:
// [
//   "Apple",
//   "Banana",
//   "Cherry"
// ]
// 27
// Cherry
```

&nbsp;

CCValue Basic Usage
-------------------

`CCValue` is a strongly typed version that supports `Read`, `Write`,
`Modify`, and `ReadOrWrite` but only for 'values'.

This shows a value being written to disk and then read back as a 'value':

```csharp
using Fmbm.IO;

var ccvalue = new CCValue<List<string>>("CCFile_Sample.txt");

// Serialize a list to disk
ccvalue.Write(new List<string> { "Apple", "Banana", "Cherry" });

// Deserialize file and get last element of list
Console.WriteLine(ccvalue.Read().Last());

// OUTPUT:
// Cherry

```

&nbsp;

Symbolic Links
--------------

The file's FullName is used as the key for synchronizing access it.  This should
work correctly across different instances of `CCFile` that access the same file
even if they are created with different relative paths or with different casing.
However if a file is accessible though a symbolic link then synchronization may
not work as expected if different instances of `CCFile` use the symbolic
link inconsistently.  E.g. if `CCFile`s are instantiated with
`/apple/banana/cherry/thefile.txt` and `/apple/sldir/thefile.txt` then access
to `thefile.txt` will not be thread-safe.

&nbsp;

ReadOrWrite
-----------

`ReadOrWrite` is a thread-safe and convenient way to set the initial content
of the file if the file does not already exist.

If the file does not already exist then the `getInitialValue` argument is
called and the result is written to the file.  If the file already exists
then `ReadOrWrite` will return the existing file content.  

```csharp
using Fmbm.IO;

var ccfile = new CCFile("CCFile_Sample.txt");

// If file does not already exist 'getInitialValue' will be called:
var result1 = ccfile.ReadOrWriteText(() => "Apple");
Console.WriteLine(result1);

// 'getInitialValue' not called because file now exists.
var result2 = ccfile.ReadOrWriteText(() => "Banana");
Console.WriteLine(result2);

// OUTPUT:
// Apple
// Apple
```

&nbsp;

Modify
------

`Modify` provides a thread-safe way to change the contents of the file.

```csharp
using Fmbm.IO;

var ccvalue = new CCValue<string[]>("CCFile_Sample.txt");

ccvalue.Write(new[] { "Cherry", "Banana", "Apple" });

Console.WriteLine(String.Join(", ", ccvalue.Read()));

ccvalue.Modify(fruit =>
{
    Array.Sort(fruit);
    return fruit;
});

Console.WriteLine(String.Join(", ", ccvalue.Read()));

// OUTPUT:
// Cherry, Banana, Apple
// Apple, Banana, Cherry
```

&nbsp;

Exists and Delete
-----------------

`Exists` indicates whether the file exists.

`Delete` deletes the file and any backup or temporary file.

```csharp
using Fmbm.IO;

var ccfile = new CCFile("CCFile_Sample.txt");
Console.WriteLine(ccfile.Exists);

ccfile.WriteText("");
Console.WriteLine(ccfile.Exists);

ccfile.Delete();
Console.WriteLine(ccfile.Exists);

// OUTPUT:
// False
// True
// False

```

&nbsp;

Archive
-------

The constructors of `CCFile` and `CCValue` take an optional `archive`
parameter.  `archive` is an Action that takes a string and a nullable
string.

`archive` is called when any write completes (including ReadOrWrite and
Modify).  The first argument is the path to the new or updated file.  The
second argument is the path to the backup file, or `null` if there is no
backup file.

```csharp
using Fmbm.IO;

void Archive(string filePath, string? backPath)
{
    var after = File.ReadAllText(filePath);
    var before = backPath is null ? "<none>" : File.ReadAllText(backPath);

    Console.Write($@"
** File Updated **
==================
Before: {before}
After: {after}
");
}

var ccvalue =
    new CCValue<Dictionary<string, string>>("CCFile_Sample.txt", Archive);

ccvalue.ReadOrWrite(() =>
    new Dictionary<string, string> { { "A", "Apple" }, { "B", "Banana" } });

ccvalue.Modify(fruit =>
{
    fruit.Add("C", "Cherry");
    return fruit;
});

// OUTPUT:

// ** File Updated **
// ==================
// Before: <none>
// After: {
//   "A": "Apple",
//   "B": "Banana"
// }

// ** File Updated **
// ==================
// Before: {
//   "A": "Apple",
//   "B": "Banana"
// }
// After: {
//   "A": "Apple",
//   "B": "Banana",
//   "C": "Cherry"
// }
```

&nbsp;

Interfaces
----------

`CCFile` implements the interface `ICCFile` which extends: `ICCBinary`,
`ICCText` and `ICCGeneric`

`CCValue` implements the interface `ICCValue`

`ICCBinary`, `ICCText`, `ICCGeneric` and `ICCValue` each defines
`Exists` and defines their own versions of:

* Read
* Write
* ReadOrWrite
* Modify

&nbsp;

Files Check
-----------

The normal write process is:

* New data is written to the _temporary_ path.
* The existing file is moved to the _backup_ path overwriting any existing
  backup.
* The _temporary_ file is moved to the _file_ path.
* Archive is called.

The _temporary_ path is the _file_ path with a `.tmp` extension appended.

The _backup_ path is the _file_ path with a `.bak` extension appended.

Before reads and writes `CCFile` checks to determine if a previous write
might be incomplete.  If the check fails then a `CCFileException` is thrown.

This table shows the write steps and shows the combinations of files that
would fail the check for an incomplete write and cause a `CCFileException`
to be thrown.

```Text
Lower case f, b: existing file.
Upper case T, F, B: updated file.
┌──────┬──────┬──────┬─────────────────────────────────────────────────────┐
│ .tmp │ file │ .bak │                                                     │
├──────┴──────┴──────┴─────────────────────────────────────────────────────┤
│                                                                          │
│                              First write                                 │
├──────┬──────┬──────┬─────────────────────────────────────────────────────┤
│      │      │      │ Pass: initial state.                                │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│  T   │      │      │ Fail: .tmp may or may no be complete.               │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│      │  F   │      │ Pass: expected state after first write.             │
├──────┴──────┴──────┴─────────────────────────────────────────────────────┤
│                                                                          │
│                              Second write                                │
├──────┬──────┬──────┬─────────────────────────────────────────────────────┤
│      │  f   │      │ Pass: initial state.                                │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│  T   │  f   │      │ Fail: .tmp may or may not be complete.              │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│  T   │      │  B   │ Fail: but both files should be complete.            │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│      │  F   │  B   │ Pass: expected state after second write.            │
├──────┴──────┴──────┴─────────────────────────────────────────────────────┤
│                                                                          │
│                              Subsequent writes:                          │
├──────┬──────┬──────┬─────────────────────────────────────────────────────┤
│      │  f   │  b   │ Pass: initial state.                                │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│  T   │  f   │  b   │ Fail: .tmp may or may not be complete.              │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│  T   │      │  B   │ Fail: but both files should be complete.            │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│      │  F   │  B   │ Pass: expected state after subsequent writes.       │
├──────┴──────┴──────┴─────────────────────────────────────────────────────┤
│                                                                          │
│            If archive function moves or deletes the .bck file:           │
├──────┬──────┬──────┬─────────────────────────────────────────────────────┤
│      │  f   │      │ Pass: initial state.                                │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│  T   │  f   │      │ Fail: .tmp may or may not be complete.              │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│  T   │      │  B   │ Fail: but both files should be complete.            │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│      │  F   │  B   │ Pass: before archive is called.                     │
├──────┼──────┼──────┼─────────────────────────────────────────────────────┤
│      │  F   │      │ Pass: normal after archive moves/deletes .bak.      │
├──────┴──────┴──────┼─────────────────────────────────────────────────────┤
│                    │                                                     │
│ Other combinations │                  ¯\_(ツ)_/¯                         │
└────────────────────┴─────────────────────────────────────────────────────┘
```

Why is it called 'CCFile'?
--------------------------

Because it's a carbon copying, conveniently converting, concurrency
conscious, crash catching file wrapper.

[Fubu]: <https://fubumvc.github.io/>
[MSGetOrAdd]: <https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.getoradd?view=net-6.0#system-collections-concurrent-concurrentdictionary-2-getoradd(-0-system-func((-0-1)))>
