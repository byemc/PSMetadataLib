using System.Data;
using System.Reflection;

namespace PSMetadataLib.PS3;

/**
 * Represents a PlayStation 3 PARAM.SFO file.
 * 
 * Much of the details for the fields and properties of this class are thanks to the PS Developer Wiki: https://www.psdevwiki.com/ps3/PARAM.SFO
 */
public class PS3ParamSFO : SfoFile
{
    /**
     * Application version, represented as a string 5 characters long in the format "XX.YY"
     *
     * Parameter name is APP_VER
     */
    public string? AppVer
    {
        get => (string?)Entries.GetValueOrDefault("APP_VER");
        set
        {
            if (value is not { Length: 5 } && value is not null)
            {
                throw new ConstraintException("APP_VER must be 5 characters long.");
            }

            if (value is null)
            {
                Entries.Remove("APP_VER");
                return;
            }
            Entries["APP_VER"] = value;
        }
    }

    /**
     * Flags effecting features of the content.
     * Documented properly at https://www.psdevwiki.com/ps3/PARAM.SFO#ATTRIBUTE, this is also where my descriptions for the enums come from.
     * Enum names are based on usage on bootable content first, and then save data, disc subfolders and finally patches.
     */
    public AttributeEnum? Attribute { get => Entries.TryGetValue("ATTRIBUTE", out var attr)
            ? (AttributeEnum)attr : null;
        set
        {
            if (value is null)
            {
                Entries.Remove("ATTRIBUTE");
                return;
            }
            Entries["ATTRIBUTE"] = (uint)value;
        }
    }

    /**
     * Specifies if the content is bootable or not. Used on games.
     *
     * Parameter name is BOOTABLE
     */
    public BootableEnum? Bootable { get => Entries.TryGetValue("BOOTABLE", out var bootable)
            ? (BootableEnum)bootable : null;
        set
        {
            if (value is null)
            {
                Entries.Remove("BOOTABLE");
                return;
            }

            Entries["BOOTABLE"] = (uint)value;
        }
    }
    
    /**
     * Specifies the category of the content, i.e. where it is placed in the XMB.
     */
    public PS3ParamCategoryEnum? Category
    {
        get
        {
            var cat = Entries.TryGetValue("CATEGORY", out var o) 
                ? (string)o : null;     // This is a string containing the value I'd like to find

            if (cat is null) return null;

            try
            {
                return (PS3ParamCategoryEnum)typeof(PS3ParamCategoryEnum)
                    .GetFields()
                    .First(f => f.IsLiteral && cat == f.GetCustomAttribute<ShortNameAttribute>()!.ShortName)
                    .GetRawConstantValue()!;
            } catch (ArgumentNullException)
            {
                return null;
            }

        }
        set
        {
            if (value is null)
            {
                Entries.Remove("CATEGORY"); return;
            }

            Entries["CATEGORY"] = value.ToShortNames();
        }
    }

    /**
     * Used in save files. Text displayed beneath SUBTITLE. Maximum 1023 characters.
     */
    public string? Detail
    {
        get => (string?)Entries.GetValueOrDefault("DETAIL");
        set
        {
            switch (value)
            {
                case { Length: > 1023 }:
                    throw new ConstraintException("DETAIL must be less than 1024 characters long.");
                case null:
                    Entries.Remove("DETAIL");
                    break;
                default:
                    Entries["DETAIL"] = value;
                    break;
            }
        }
    }

    /**
     * Orders content vertically in the XMB.
     *
     * Parameter is ITEM_PRIORITY
     */
    public uint? ItemPriority
    {
        get => (uint?)Entries.GetValueOrDefault("ITEM_PRIORITY");
        set
        {
            if (value is null)
            {
                Entries.Remove("ITEM_PRIORITY");
                return;
            }

            Entries["ITEM_PRIORITY"] = value;
        }
    }

    /**
     * Language used by trophies(?). Doesn't include every language supported by this field due to the PS3 not supporting them here.
     *
     * Parameter: LANG
     */
    public LanguagesEnum? Lang { get => Entries.TryGetValue("LANG", out var bootable)
            ? (LanguagesEnum)bootable : null;
        set
        {
            if (value is null)
            {
                Entries.Remove("LANG");
                return;
            }

            Entries["LANG"] = (uint)value;
        }
    }
    
    /**
     * License text of content (Used by HDD Games)
     *
     * Parameter: LICENSE
     */
    public string? License
    {
        get => (string?)Entries.GetValueOrDefault("LICENSE");
        set
        {
            switch (value)
            {
                case { Length: > 511 }:
                    throw new ConstraintException("LICENSE must be less than 511 characters long.");
                case null:
                    Entries.Remove("LICENSE");
                    break;
                default:
                    Entries["LICENSE"] = value;
                    break;
            }
        }
    }

    /**
     * Used for parental controls
     */
    public uint? ParentalLevel
    {
        get => (uint?)Entries.GetValueOrDefault("PARENTAL_LEVEL");
        set
        {
            switch (value)
            {
                case null:
                    Entries.Remove("PARENTAL_LEVEL");
                    break;
                default:
                    Entries["PARENTAL_LEVEL"] = value;
                    break;
            }
        }
    }

    /**
     * Defines what video modes content supports.
     */
    public ResolutionEnum? Resolution { get => Entries.TryGetValue("RESOLUTION", out var bootable)
        ? (ResolutionEnum)bootable : null;
        set
        {
            if (value is null)
            {
                Entries.Remove("RESOLUTION");
                return;
            }

            Entries["RESOLUTION"] = (uint)value;
        }
    }

    /**
     * Defines what sound modes the content supports
     */
    public SoundFormatEnum? SoundFormat { get => Entries.TryGetValue("SOUND_FORMAT", out var bootable)
            ? (SoundFormatEnum)bootable : null;
        set
        {
            if (value is null)
            {
                Entries.Remove("SOUND_FORMAT");
                return;
            }

            Entries["SOUND_FORMAT"] = (uint)value;
        }
    }
    
    /**
     * Subtitle of content.
     */
    public string? SubTitle
    {
        get => (string?)Entries.GetValueOrDefault("SUB_TITLE");
        set
        {
            if (value is null)
            {
                Entries.Remove("SUB_TITLE");
            }
            else if (value is not { Length: >= 128 })
            {
                Entries["SUB_TITLE"] = value;
            }
            else
            {
                throw new ConstraintException("SUB_TITLE must be less than 128 characters long.");
            }
        }
    }
    
    /**
     * Title of the content. Max of 127 characters.
     * Localised titles might be in the Entries parameter as TITLE_xx, where xx is the regional code.
     */
    public string? Title
    {
        get => (string?)Entries.GetValueOrDefault("TITLE");
        set
        {
            if (value is null)
            {
                Entries.Remove("TITLE");
            }
            else if (value is not { Length: >= 128 })
            {
                Entries["TITLE"] = value;
            }
            else
            {
                throw new ConstraintException("TITLE must be less than 128 characters long.");
            }
        }
    }
    
    /**
     * Title ID of the content. Max of 15 characters.
     */
        public string? TitleId
        {
            get => (string?) Entries.GetValueOrDefault("TITLE_ID");
            set
            {
                switch (value)
                {
                    case null:
                        Entries.Remove("TITLE_ID");
                        break;
                    case { Length: < 16 }:
                        throw new ConstraintException("TITLE_ID must be less than 16 characters long.");
                    default:
                        Entries["TITLE_ID"] = value;
                        break;
                }
            }
        }
    
    /**
     * Create PS3ParamSFO file using information from a PARAM.SFO file given in `path`
     */
    public PS3ParamSFO(string path)
    {
        // Loads the entries from the PARAM.SFO into Entries.
        Load(path);

        if (Length <= 0)
        {
            throw new FileLoadException("Could not find any parameters in the PARAM.SFO file.");
        }
    }
}