namespace PSMetadataLib.Types;

//TODO: VALIDATION
public class PublisherId(string id)
{
    public string Id { get; set; } = id;

    public override string ToString()
    {
        return Id;
    }
}

public class NPTitleId(string id)
{
    public string Id { get; set; } = id;
    
    public override string ToString()
    {
        return Id;
    }
}

public class ServiceId
{
    public PublisherId PublisherId { get; set; }
    public NPTitleId NpTitleId { get; set; }

    public string Id {
        get => $"{PublisherId}-{NpTitleId}";
        set
        {
            var splittedId = value.Split("-");
            PublisherId = new PublisherId(splittedId[0]);
            NpTitleId = new NPTitleId(splittedId[1]);
        }
    }
    
    public ServiceId(string id)
    {
        var splittedId = id.Split("-");
        PublisherId = new PublisherId(splittedId[0]);
        NpTitleId = new NPTitleId(splittedId[1]);
    }

    public ServiceId(PublisherId publisherId, NPTitleId npTitleId)
    {
        PublisherId = publisherId;
        NpTitleId = npTitleId;
    }
    
    public override string ToString()
    {
        return Id;
    }
}

public class EntitlementLabel
{
    private string _label = "";

    public string Label
    {
        get => _label;
        set
        {
            _label = value.ToUpper()[..16];
        }
    }

    public EntitlementLabel(string label)
    {
        Label = label;
    }
    
    public override string ToString()
    {
        return Label;
    }
}

public class ContentId
{
    public ServiceId? ServiceId { get; set; }
    public EntitlementLabel? EntitlementLabel { get; set; }

    public string Id {
        get => $"{ServiceId}-{EntitlementLabel}";
        set
        {
            var splittedId = value.Split("-");
            ServiceId = new ServiceId(splittedId[0]);
            EntitlementLabel = new EntitlementLabel(splittedId[1]);
        }
    }
    
    public ContentId(string? id)
    {
        if (id is null)
            return;
        var splittedId = id.Split("-");
        ServiceId = new ($"{splittedId[0]}-{splittedId[1]}");
        EntitlementLabel = new (splittedId[2]);
    }

    public ContentId(ServiceId service, EntitlementLabel entitlement)
    {
        ServiceId = service;
        EntitlementLabel = entitlement;
    }
    
    public override string ToString()
    {
        return Id;
    }

    public static implicit operator ContentId(string? value)
    {
        return new ContentId(value);
    }
}