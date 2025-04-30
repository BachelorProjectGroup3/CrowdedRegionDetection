using System.Data;
using System.Net;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            Console.WriteLine($"This is raspberryPi: {raspberryPi}");

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

            // Test random MacAddress and see if there are 3??
            var foundDevices = _context.RaspData.Where(d => d.MacAddress == raspOutputData.Events[0].MacAddress);

            if (foundDevices.Count() == 3)
            {
                var foundDeviceslist = await foundDevices.ToListAsync();

                foreach (var device in foundDeviceslist)
                {
                    var raspSpecificEvents = await _context.RaspData
                        .Where(rasp => rasp.RaspId == device.RaspId)
                        .ToListAsync();

                    var eventList = new List<RaspEvent>();

                    foreach (var raspEvent in raspSpecificEvents)
                    {
                        eventList.Add(new RaspEvent { MacAddress = raspEvent.MacAddress, Rssi = raspEvent.Rssi, UnixTimestamp = now });
                    }

                    var rasp = await _context.RaspberryPi.FindAsync(device.RaspId);

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

                Console.WriteLine(_context);

                await _context.SaveChangesAsync();
                _circleUtils.WipeData();
                await this.WipeRaspData();
            }

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
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