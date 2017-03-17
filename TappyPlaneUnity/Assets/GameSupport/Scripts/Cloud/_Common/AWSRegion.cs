using Amazon;

public enum AWSRegionEnum
{
    None,
    APNortheast1,
    APNortheast2,
    APSouth1,
    APSoutheast1,
    APSoutheast2,
    CNNorth1,
    EUCentral1,
    EUWest1,
    SAEast1,
    USEast1,
    USEast2,
    USGovCloudWest1,
    USWest1,
    USWest2
}

public static class AWSRegionHelpers
{
    public static RegionEndpoint GetByEnum(AWSRegionEnum regionEnum)
    {
        switch(regionEnum)
        {
            case AWSRegionEnum.APNortheast1: return RegionEndpoint.APNortheast1;
            case AWSRegionEnum.APNortheast2: return RegionEndpoint.APNortheast2;
            case AWSRegionEnum.APSouth1:     return RegionEndpoint.APSouth1;
            case AWSRegionEnum.APSoutheast1: return RegionEndpoint.APSoutheast1;
            case AWSRegionEnum.APSoutheast2: return RegionEndpoint.APSoutheast2;
            case AWSRegionEnum.CNNorth1: return RegionEndpoint.CNNorth1;
            case AWSRegionEnum.EUCentral1: return RegionEndpoint.EUCentral1;
            case AWSRegionEnum.EUWest1: return RegionEndpoint.EUWest1;
            case AWSRegionEnum.SAEast1: return RegionEndpoint.SAEast1;
            case AWSRegionEnum.USEast1: return RegionEndpoint.USEast1;
            case AWSRegionEnum.USEast2: return RegionEndpoint.USEast2;
            case AWSRegionEnum.USGovCloudWest1: return RegionEndpoint.USGovCloudWest1;
            case AWSRegionEnum.USWest1: return RegionEndpoint.USWest1;
            case AWSRegionEnum.USWest2: return RegionEndpoint.USWest2;
        }

        return null;
    }
}