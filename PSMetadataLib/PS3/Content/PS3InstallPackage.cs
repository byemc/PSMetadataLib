namespace PSMetadataLib.PS3.Content;

public class PS3InstallPackage(string location) : PS3Content(location)
{
    // Parameters

    /**
     * This category can only ever be IP for this content type.
     */
    public PS3ParamCategoryEnum Category => PS3ParamCategoryEnum.InstallPackage;

    public int ParentalLevel
    {
        get => (int?)ParamSfo.ParentalLevel ?? 1;
        set => ParamSfo.ParentalLevel = (uint)value;
    }
}