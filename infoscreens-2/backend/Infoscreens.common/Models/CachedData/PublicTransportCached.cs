using Infoscreens.Common.Helpers;
using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.CachedData
{
    public class PublicTransportCached
    {
        public StationCached Station { get; set; }
        public IEnumerable<JourneyCached> StationBoard { get; set; }
        public IEnumerable<DestinationGroup> DestinationGroups { get; set; }


        public PublicTransportCached(StationCached station, IEnumerable<JourneyCached> stationBoard, IEnumerable<DestinationGroup> destinationGroups)
        {
            Station = station;
            StationBoard = stationBoard;
            DestinationGroups = destinationGroups;
        }
    }

    public class StationCached
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public StationCached(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class JourneyCached
    {
        public string LineNumber { get; set; }
        public string TransportType { get; set; }
        public string Destination { get; set; }
        public string DepartureTimestamp { get; set; }
        public string Delay { get; set; }

        public JourneyCached(string lineNumber, string transportType, string destination, DateTimeOffset departureTimestamp, string delay)
        {
            LineNumber = lineNumber;
            TransportType = transportType;
            Destination = destination;
            DepartureTimestamp = DateHelper.ToIsoDateTimeString(departureTimestamp);
            Delay = delay;
        }
    }

    public class DestinationGroup
    {
        public string Name { get; set; }
        public IEnumerable<JourneyCriteria> Criterias { get; set; }
    }

    public class JourneyCriteria
    {
        public string LineNumber { get; set; }
        public string Destination { get; set; }
    }
}
