RockFile
========

File persistence with the performance and flexibility of a rock!

Security: Paths may be revealed.

```Text
┌──────┬──────┬──────┬──────┬──────────────────────────────────────────────┐
│ .lck │ .tmp │ file │ .bak │ Recommendation                               │
├──────┼──────┼──────┼──────┼──────────────────────────────────────────────┤
│      │      │      │      │ Normal: before any data written              │
├──────┼──────┼──────┼──────┼──────────────────────────────────────────────┤
│      │      │  X   │      │ Normal: after first write                    │
├──────┼──────┼──────┼──────┼──────────────────────────────────────────────┤
│      │      │  X   │  X   │ Normal: after second write                   │
├──────┼──────┼──────┼──────┼──────────────────────────────────────────────┤
│  X   │      │      │      │ 1) Delete .lck file                          │
├──────┼──────┼──────┼──────┤                                              │
│  X   │      │  X   │      │ Failed before write started, or after write  │
├──────┼──────┼──────┼──────┤ completed. 'file' should be good.            │
│  X   │      │  X   │  X   │                                              │
├──────┼──────┼──────┼──────┼──────────────────────────────────────────────┤
│  X   │  X   │      │      │ 1) Delete .tmp OR if sure contents of .tmp   │
│      │      │      │      │    are good then manually remove .bak, move  │
│──────┼──────┼──────┼──────│    'file' to .bak, and move .tmp to 'file'.  │
│  X   │  X   │  X   │      │                                              │
│      │      │      │      │ 2) Delete .lck file                          │
│──────┼──────┼──────┼──────│                                              │
│  X   │  X   │  X   │  X   │ May have failed while writing new data to    │
│      │      │      │      │ .tmp file. Contents may be incomplete.       │
├──────┼──────┼──────┼──────┼──────────────────────────────────────────────┤
│  X   │  X   │      │  X   │ 1) Move .tmp to 'file'                       │
│      │      │      │      │                                              │
│      │      │      │      │ 2) Delete .lck file                          │
│      │      │      │      │                                              │
│      │      │      │      │ This state should only arise if write to     │
│      │      │      │      │ .tmp completed.  Failure may be caused by    │
│      │      │      │      │ by a call an archiver.                       │
├──────┴──────┴──────┴──────┼──────────────────────────────────────────────┤
│    Other combinations     │                ¯\_(ツ)_/¯                    │
└───────────────────────────┴──────────────────────────────────────────────┘
```
