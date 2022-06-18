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
* Prevents concurrent access to a file (including from different instances
  of `CCFile`).
* Only writes to file after with all the data is available (to prevent
  partial writes when an exception is thrown while creating the data).
* Checks for evidence of previous incomplete writes.
* `Modify` method ensures file is not changed by a separte process between
  reading and then writing new data.

Limitations:

* Although intended to be robust, this is a first release and so may not be
  as reliable as hoped and may have dreadful bugs.
* Concurrency checking is only when using CCFile.  (Although exclusive file 
  access should prevent concurrent writes.)
* It's not the fastest way to write to file as it assembles a complete
  `byte[]` before writing.

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

Basic Usage
-----------

`CCFile` supports `Read`, `write`, `Modify`, and `ReadOrWrite` for each of
`bytes[]`, `string` and 'values'.  These example just show `string`.
Examples with 'values' are below:

```C#
using Fmbm.IO;



```

----------------------------------------------------------------------------

intefaces

----------------------------------------------------------------------------

deleting

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

Because it's conveniently converting, concurrency concious, carbon copying,
and crash catching.

[Fubu]: <https://fubumvc.github.io/>
[MSGetOrAdd]: <https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.getoradd>
