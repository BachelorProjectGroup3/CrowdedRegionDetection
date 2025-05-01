using System.Data;
using System.Net;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CrowdedBackend.Helpers;

public class DetectedDeviceHelper
{
    private readonly MyDbContext _context;
    private CircleUtils _circleUtils;

    public DetectedDeviceHelper(MyDbContext context, CircleUtils circleUtils)
    {
        this._context = context;
        this._circleUtils = circleUtils;
    }

    public async Task<RaspOutputData> HandleRaspPostRequest(RaspOutputData raspOutputData, long now)
    {
        try
        {
            var raspberryPi = await _context.RaspberryPi.FindAsync(raspOutputData.Id);

            if (raspberryPi == null)
            {
                throw new HttpRequestException(HttpRequestError.Unknown, message: "Raspberry Pi not found", statusCode: HttpStatusCode.InternalServerError);
            }

            foreach (var raspEvent in raspOutputData.Events)
            {
                await PostRaspData(new RaspData
                {
                    MacAddress = raspEvent.MacAddress,
                    RaspId = raspberryPi.RaspberryPiID,
                    Rssi = raspEvent.Rssi,
                    UnixTimestamp = now
                });
            }

            var uniqueIds = await _context.RaspData.Select(x => x.RaspId).Distinct().ToListAsync();
            
            if (uniqueIds.Count == 3)
            {
                var macsWithExactly3RaspIds = await _context.RaspData
                    .Where(rd => uniqueIds.Contains(rd.RaspId))
                    .GroupBy(rd => rd.MacAddress)
                    .Where(g => g.Select(rd => rd.RaspId).Distinct().Count() == 3)
                    .Select(g => g.Key)
                    .ToListAsync();
                
                foreach (var id in uniqueIds)
                {
                    var raspSpecificEvents = await _context.RaspData
                        .Where(x => x.RaspId == id && macsWithExactly3RaspIds.Contains(x.MacAddress))
                        .GroupBy(x => x.MacAddress)
                        .Select(g => g.OrderByDescending(x => x.UnixTimestamp).First())
                        .ToListAsync();

                    var eventList = new List<RaspEvent>();

                    foreach (var raspEvent in raspSpecificEvents)
                    {
                        eventList.Add(new RaspEvent { 
                            MacAddress = raspEvent.MacAddress, 
                            Rssi = raspEvent.Rssi, 
                            UnixTimestamp = now 
                        });
                    }

                    var rasp = await _context.RaspberryPi.FindAsync(id);

                    if (rasp == null)
                    {
                        throw new HttpRequestException(HttpRequestError.Unknown, message: "Failed to find raspberryPi in raw data table", statusCode: HttpStatusCode.InternalServerError);
                    }

                    _circleUtils.AddData(new RaspOutputData { Events = eventList }, new Point(rasp.RaspX, rasp.RaspY));
                }

                var points = _circleUtils.CalculatePosition();
                foreach (var point in points)
                {
                    _context.Add(new DetectedDevice { VenueID = raspberryPi.VenueID, DeviceX = point.X, DeviceY = point.Y, Timestamp = now });
                }

                await _context.SaveChangesAsync();
                _circleUtils.WipeData();
                await this.WipeRaspData();
            }

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
        }

        return raspOutputData;
    }

    public async Task<RaspData> PostRaspData(RaspData raspData)
    {
        _context.RaspData.Add(raspData);
        await _context.SaveChangesAsync();

        return raspData;
    }

    public async Task<IQueryable<RaspData>> WipeRaspData()
    {
        var raspData = _context.RaspData.Where(i => true);

        foreach (var rasp in raspData)
        {
            _context.RaspData.Remove(rasp);
        }
        await _context.SaveChangesAsync();

        return raspData;
    }

}