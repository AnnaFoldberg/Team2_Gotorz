using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared;
using Gotorz.Server.Services;

namespace Gotorz.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private IFlightService _flightService;
    
    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet("flights/auto-complete")]
    public async Task<IActionResult> GetAutoCompleteAsync()
    {
        var airport = await _flightService.GetAutoCompleteAsync();

        if ( airport.Count > 1 )
        {
            return BadRequest("Multiple airports were found");
        }

        else if ( airport.Count == 0 )
        {
            return BadRequest("No airport was found");
        }

        else if ( airport == null )
        {
            return BadRequest("Something went wrong");
        }

        return Ok(airport);

        // Vi vil gerne gemme entityId (=> originId / destinationId) og skyId (=> origin / destination)
    }

    [HttpGet("flights/one-way")]
    public async Task<IActionResult> GetOneWay()
    {
    }
}

// Example response:
// {
//     "data": {
//         "context": {
//             "sessionId": "UNFOCUSED_SESSION_ID",
//             "status": "complete",
//             "totalResults": 0
//         },
//         "differentDestination": {
//             "context": {
//                 "sessionId": "UNFOCUSED_SESSION_ID",
//                 "status": "complete",
//                 "totalResults": 0
//             },
//             "location": {
//                 "id": "29475437",
//                 "name": "United States",
//                 "skyCode": "US",
//                 "type": "Nation"
//             }
//         },
//         "flightQuotes": {
//             "buckets": [
//                 {
//                     "id": "CHEAPEST_FLIGHT_QUOTES",
//                     "label": "Cheapest flights",
//                     "resultIds": [
//                         "{bl}:202503250257*I*EWR*HNL*20250428*vaya*NK",
//                         "{bl}:202503231653*I*EWR*HNL*20250429*smtf*AS",
//                         "{bl}:202503250443*I*EWR*HNL*20250427*aaus*AA",
//                         "{bl}:202503251205*I*EWR*HNL*20250425*dhop*NK",
//                         "{bl}:202503250256*I*EWR*HNL*20250426*dhop*NK",
//                         "{bl}:202503220243*I*JFK*HNL*20250422*wfus*AS",
//                         "{bl}:202503250256*I*JFK*HNL*20250423*cust*AS",
//                         "{bl}:202503231728*I*JFK*HNL*20250430*wfus*AS",
//                         "{bl}:202503220243*D*JFK*HNL*20250422*wfus*AS"
//                     ]
//                 },
//                 {
//                     "id": "DIRECT_FLIGHT_QUOTES",
//                     "label": "Direct flights",
//                     "resultIds": [
//                         "{bl}:202503220243*D*JFK*HNL*20250422*wfus*AS",
//                         "{bl}:202503231653*D*JFK*HNL*20250429*smtf*AS",
//                         "{bl}:202503221823*D*JFK*HNL*20250426*smtf*AS",
//                         "{bl}:202503231728*D*JFK*HNL*20250430*wfus*AS",
//                         "{bl}:202503230901*D*JFK*HNL*20250419*wfus*AS",
//                         "{bl}:202503250443*D*JFK*HNL*20250427*smtf*AS",
//                         "{bl}:202503241747*D*JFK*HNL*20250428*smtf*AS",
//                         "{bl}:202503242045*D*JFK*HNL*20250421*smtf*AS",
//                         "{bl}:202503251205*D*JFK*HNL*20250425*smtf*AS"
//                     ]
//                 }
//             ],
//             "context": {
//                 "sessionId": "UNFOCUSED_SESSION_ID",
//                 "status": "complete",
//                 "totalResults": 17
//             },
//             "months": [
//                 {
//                     "month": 3,
//                     "monthLabel": "Mar",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 4,
//                     "monthLabel": "Apr",
//                     "selected": true,
//                     "year": 2025
//                 },
//                 {
//                     "month": 5,
//                     "monthLabel": "May",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 6,
//                     "monthLabel": "Jun",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 7,
//                     "monthLabel": "Jul",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 8,
//                     "monthLabel": "Aug",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 9,
//                     "monthLabel": "Sep",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 10,
//                     "monthLabel": "Oct",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 11,
//                     "monthLabel": "Nov",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 12,
//                     "monthLabel": "Dec",
//                     "selected": false,
//                     "year": 2025
//                 },
//                 {
//                     "month": 1,
//                     "monthLabel": "Jan",
//                     "selected": false,
//                     "year": 2026
//                 },
//                 {
//                     "month": 2,
//                     "monthLabel": "Feb",
//                     "selected": false,
//                     "year": 2026
//                 }
//             ],
//             "results": [
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-30",
//                             "localDepartureDateLabel": "Wed, Apr 30",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$259",
//                         "rawPrice": 259.0
//                     },
//                     "id": "{bl}:202503231728*I*JFK*HNL*20250430*wfus*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-22",
//                             "localDepartureDateLabel": "Tue, Apr 22",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$266",
//                         "rawPrice": 266.0
//                     },
//                     "id": "{bl}:202503220243*D*JFK*HNL*20250422*wfus*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-30",
//                             "localDepartureDateLabel": "Wed, Apr 30",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$269",
//                         "rawPrice": 269.0
//                     },
//                     "id": "{bl}:202503231728*D*JFK*HNL*20250430*wfus*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-29",
//                             "localDepartureDateLabel": "Tue, Apr 29",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$269",
//                         "rawPrice": 269.0
//                     },
//                     "id": "{bl}:202503231653*D*JFK*HNL*20250429*smtf*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-25",
//                             "localDepartureDateLabel": "Fri, Apr 25",
//                             "originAirport": {
//                                 "id": "95565059",
//                                 "name": "EWR",
//                                 "skyCode": "EWR",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$239",
//                         "rawPrice": 239.0
//                     },
//                     "id": "{bl}:202503251205*I*EWR*HNL*20250425*dhop*NK",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-23",
//                             "localDepartureDateLabel": "Wed, Apr 23",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$257",
//                         "rawPrice": 257.0
//                     },
//                     "id": "{bl}:202503250256*I*JFK*HNL*20250423*cust*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-27",
//                             "localDepartureDateLabel": "Sun, Apr 27",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$270",
//                         "rawPrice": 270.0
//                     },
//                     "id": "{bl}:202503250443*D*JFK*HNL*20250427*smtf*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-26",
//                             "localDepartureDateLabel": "Sat, Apr 26",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$269",
//                         "rawPrice": 269.0
//                     },
//                     "id": "{bl}:202503221823*D*JFK*HNL*20250426*smtf*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-25",
//                             "localDepartureDateLabel": "Fri, Apr 25",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$270",
//                         "rawPrice": 270.0
//                     },
//                     "id": "{bl}:202503251205*D*JFK*HNL*20250425*smtf*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-28",
//                             "localDepartureDateLabel": "Mon, Apr 28",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$270",
//                         "rawPrice": 270.0
//                     },
//                     "id": "{bl}:202503241747*D*JFK*HNL*20250428*smtf*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-26",
//                             "localDepartureDateLabel": "Sat, Apr 26",
//                             "originAirport": {
//                                 "id": "95565059",
//                                 "name": "EWR",
//                                 "skyCode": "EWR",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$239",
//                         "rawPrice": 239.0
//                     },
//                     "id": "{bl}:202503250256*I*EWR*HNL*20250426*dhop*NK",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-27",
//                             "localDepartureDateLabel": "Sun, Apr 27",
//                             "originAirport": {
//                                 "id": "95565059",
//                                 "name": "EWR",
//                                 "skyCode": "EWR",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$234",
//                         "rawPrice": 234.0
//                     },
//                     "id": "{bl}:202503250443*I*EWR*HNL*20250427*aaus*AA",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-22",
//                             "localDepartureDateLabel": "Tue, Apr 22",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$240",
//                         "rawPrice": 240.0
//                     },
//                     "id": "{bl}:202503220243*I*JFK*HNL*20250422*wfus*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-21",
//                             "localDepartureDateLabel": "Mon, Apr 21",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$270",
//                         "rawPrice": 270.0
//                     },
//                     "id": "{bl}:202503242045*D*JFK*HNL*20250421*smtf*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-29",
//                             "localDepartureDateLabel": "Tue, Apr 29",
//                             "originAirport": {
//                                 "id": "95565059",
//                                 "name": "EWR",
//                                 "skyCode": "EWR",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$232",
//                         "rawPrice": 232.0
//                     },
//                     "id": "{bl}:202503231653*I*EWR*HNL*20250429*smtf*AS",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": false,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-28",
//                             "localDepartureDateLabel": "Mon, Apr 28",
//                             "originAirport": {
//                                 "id": "95565059",
//                                 "name": "EWR",
//                                 "skyCode": "EWR",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$229",
//                         "rawPrice": 229.0
//                     },
//                     "id": "{bl}:202503250257*I*EWR*HNL*20250428*vaya*NK",
//                     "type": "FLIGHT_QUOTE"
//                 },
//                 {
//                     "content": {
//                         "direct": true,
//                         "outboundLeg": {
//                             "destinationAirport": {
//                                 "id": "95673827",
//                                 "name": "HNL",
//                                 "skyCode": "HNL",
//                                 "type": "Airport"
//                             },
//                             "localDepartureDate": "2025-04-19",
//                             "localDepartureDateLabel": "Sat, Apr 19",
//                             "originAirport": {
//                                 "id": "95565058",
//                                 "name": "JFK",
//                                 "skyCode": "JFK",
//                                 "type": "Airport"
//                             }
//                         },
//                         "price": "$269",
//                         "rawPrice": 269.0
//                     },
//                     "id": "{bl}:202503230901*D*JFK*HNL*20250419*wfus*AS",
//                     "type": "FLIGHT_QUOTE"
//                 }
//             ]
//         }
//     },
//     "status": "success",
//     "token": "eyJjYyI6ICJlY29ub215IiwgImEiOiAxLCAiYyI6IDAsICJpIjogMCwgImwiOiBbWyJOWUNBIiwgIkhOTCIsICJhbnl0aW1lIl1dfQ=="
// }