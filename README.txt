PSMetadataLib
~~~~~~~~~~~~~

PSMetadataLib is a work-in-progress library for parsing files related to metadata on PlayStation systems, currently focused on the PlayStation 3.

The following is planned for the short term:
- [ ] PlayStation 3 support
    - [ ] Properly parse, modify and write PARAM.SFO files
    - [ ] Properly parse, modify and write PS3_DISK.SFB files
    - [ ] Identify assets related to the current PARAM.SFO file.
        - [ ] Images (like PIC1.PNG, ICON0.PNG, etc)
        - [ ] Audio / Video (SND0.AT3, etc)

Implementing the above will also help with parsing related files on PSP, PS Vita and PS4. In the long term I hope to support all metadata-related things across all PlayStation generations (pending checking what I can even do for PSX/PS2.. save data maybe?)

A use case of this could be identifying PlayStation games without using an internet connection, using the data provided by the game directly.

This project is licensed under the MIT license. Please see more details in the LICENSE.txt file.

I would like to thank the contributors of the PS3 Dev Wiki (https://www.psdevwiki.com/ps3/) for documenting so much about the file formats PlayStation systems use.
