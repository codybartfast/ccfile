CCFile
======

Read, write and serialize to file - simply and robustly.

Features:

* Read and write both text and bytes (similar to `File.ReadAllText` etc.)
* Serialize objects and values in the same manner as writing text or bytes.
* Convenience `ReadOrWrite` method for initialization (similar to
  [`ConcurrentDictionary.GetOrAdd`][MSGetOrAdd]).
* Automatically creates a backup of an existing file before writting.
* Allows a separate copy of updated files to be created for archiving or
  versioning.
* Thread-safe, prevents concurrent access to a file (including from 
  different instances of `CCFile`).
* Only writes to file after with all the data is available (to prevent
  partial writes when an exception is thrown while creating the data).
* Checks for evidence of previous incomplete writes.
* `Modify` method ensures file is not changed by a separte process between
  reading and then writing new data.

(Some) Limitations:

* Although intended to be robust, this is a first release and so may not be
  as reliable as hoped and may have dreadful bugs.
* Concurrency checking is only when using CCFile.  (Although exclusive file
  access should prevent concurrent writes.)
* It's not the fastest way to write to file as it assembles a complete
  `byte[]` before writing.
* If an incomplete write is discovered then it makes no attempt to repair
  or to choose which of any existing files should be restored.

For Me, By Me (FMBM)
--------------------

This was created primarily for use by the author.  It has only been tested
in limited envirnoments.  It is intended for getting ad-hoc
applications up and running quickly.  It probably is not suitable for
complex, production, nor evolving projects.  (The name is inspired by the
[Fubu][Fubu], _For Us, By Us_, project, but there is no other connection.)

----------------------------------------------------------------------------

Contents
--------

XXX

----------------------------------------------------------------------------

CCFile Basic Usage
------------------

`CCFile` supports `Read`, `Write`, `Modify`, and `ReadOrWrite` for each of
`bytes[]`, `string` and 'values'.  

This shows a value being written to disk and then read back as a `string`,
as a `byte[]` and as a 'value':

```C#
using Fmbm.IO;

// Create new CCFile
var ccfile = new CCFile("CCFile_Sample.txt");

// Serialize a list to disk
ccfile.WriteValue(new List<string> { "Apple", "Banana", "Cherry" });

// Read file contents as text
Console.WriteLine(ccfile.ReadText());

// Read file contents as bytes
Console.WriteLine(ccfile.ReadBytes()!.Length);

// Deserialize file and get last element of list
Console.WriteLine(ccfile.ReadValue<List<string>>()!.Last());

// OUTPUT:
// ["Apple","Banana","Cherry"]
// 27
// Cherry
```

CCValue Basic Usage
-------------------

`CCValue` is a stongly typed version that supports `Read`, `Write`, 
`Modify`, and `ReadOrWrite` but only for 'values'.

This shows a value being written to disk and then read back as a 'value':

```C#
using Fmbm.IO;

var ccvalue = new CCValue<List<string>>("CCFile_Sample.txt");

// Serialize a list to disk
ccvalue.Write(new List<string> { "Apple", "Banana", "Cherry" });

// Deserialize file and get last element of list
Console.WriteLine(ccvalue.Read()!.Last());

// OUTPUT:
// Cherry

```

----------------------------------------------------------------------------

ReadOrWrite
-----------

`ReadOrWrite` is a thread safe and convenient way to set the initial content
of the file if the file does not already exist.

If the file does not already exist then it will call the provided 
`getInitialValue` argument and write its result to the file and will return
that result.  If the file already exists then `ReadOrWrite` will return the
existing content or value.  

```C#
using Fmbm.IO;

var ccfile = new CCFile("CCFile_Sample.txt");


// Assuning the file does not alread exist 'getInitialValue' will be called:
var result1 = ccfile.ReadOrWriteText(() => "Apple");
Console.WriteLine(result1);

// 'getInitialValue' not called because file does exist now.
var result2 = ccfile.ReadOrWriteText(() => "Banana");
Console.WriteLine(result2);

// OUTPUT:
// Apple
// Apple
```

----------------------------------------------------------------------------

Modify
------

`Modify` provides a thread safe way to change the contents of the file.



----------------------------------------------------------------------------

archive

----------------------------------------------------------------------------

intefaces

----------------------------------------------------------------------------

deleting

----------------------------------------------------------------------------

bang nulls

----------------------------------------------------------------------------

read returns null

deserialize as array?
search and replace

----------------------------------------------------------------------------

how writes
checks fail

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
│  T   │      │      │ Fail: .tmp may or may no be.                        │
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
concious, and crash catching file wrapper.

[Fubu]: <https://fubumvc.github.io/>
[MSGetOrAdd]: <https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.getoradd?view=net-6.0#system-collections-concurrent-concurrentdictionary-2-getoradd(-0-system-func((-0-1)))>
