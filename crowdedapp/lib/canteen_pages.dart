import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:flutter_dotenv/flutter_dotenv.dart';
// Hide ConnectionState from signalr_core to resolve ambiguity
import 'package:signalr_core/signalr_core.dart' hide ConnectionState;


final String backendUrl = dotenv.env['BackendURL'] ?? 'http://localhost:3000';
class CanteenPage extends StatefulWidget {
  final String title;
  const CanteenPage({super.key, required this.title});

  @override
  State<CanteenPage> createState() => _CanteenPageState();
}

class _CanteenPageState extends State<CanteenPage> {
  HubConnection? _hubConnection;
  int? _latestTimestamp;
  // Add key for FutureBuilder to force refresh
  final GlobalKey _futureBuilderKey = GlobalKey();
  // Track if we need to refresh
  bool _needsRefresh = false;

  @override
  void initState() {
    super.initState();
    _initSignalR();
    _fetchLatestTimestamp();
  }

  Future<void> _initSignalR() async {
    try {
      _hubConnection = HubConnectionBuilder()
          .withUrl(
            '$backendUrl/hubs/detecteddevices',
            HttpConnectionOptions(
              logging: (level, message) => print('SignalR $level: $message'),
              skipNegotiation: true,
              transport: HttpTransportType.webSockets,
            ),
          )
          .withAutomaticReconnect()
          .build();

      _hubConnection?.onclose((error) => 
          print('Connection closed: ${error?.toString() ?? "No error"}'));

      _hubConnection?.on('NewDevicesDetected', (arguments) {
        print('Received NewDevicesDetected: $arguments');
        // The backend sends: { Devices: [{ X, Y, Timestamp }] }
        if (arguments != null && arguments.isNotEmpty) {
          final devices = arguments[0]['Devices'] as List<dynamic>;
          if (devices.isNotEmpty) {
            final newTimestamp = devices[0]['Timestamp'] as int;
            // Only update state if the timestamp actually changed
            if (newTimestamp != _latestTimestamp) {
              setState(() {
                _latestTimestamp = newTimestamp;
                // Force refresh by setting flag and updating state
                _needsRefresh = true;
              });
            }
          }
        }
      });

      print('Starting connection to SignalR hub...');
      await _hubConnection?.start();
      print('Connected to SignalR hub successfully!');
    } catch (e) {
      print('Error initializing SignalR: $e');
    }
  }

  // Renamed to better reflect its purpose
  Future<void> _fetchLatestTimestamp() async {
    // In a real app, you might fetch the absolute latest timestamp from an endpoint
    // For now, just setting it to now to trigger the initial FutureBuilder load
    final now = DateTime.now().millisecondsSinceEpoch;
    setState(() {
      _latestTimestamp = now;
    });
  }

  // Modified to return Future<Image?> for FutureBuilder
  Future<Image?> _fetchHeatmapImage(int timestamp) async {
    try {
      final response = await http.get(
        Uri.parse('$backendUrl/api/DetectedDevices/getHeatmapAtSpecificTime/$timestamp'),
      );
      if (response.statusCode == 200) {
        final bytes = base64Decode(response.body);
        return Image.memory(bytes, fit: BoxFit.contain);
      } else {
        print('Error fetching heatmap: Status ${response.statusCode}');
        return null;
      }
    } catch (e) {
      print('Error fetching heatmap: $e');
      return null;
    } finally {
      // Reset the refresh flag after fetch attempt
      if (_needsRefresh) {
        setState(() {
          _needsRefresh = false;
        });
      }
    }
  }

  @override
  void dispose() {
    _hubConnection?.stop();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
          colors: [Colors.blue.shade900, Colors.blue.shade400],
        ),
      ),
      child: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SizedBox(height: 50),
            Text(
              widget.title,
              style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold, color: Colors.white),
            ),
            SizedBox(height: 10),
            Text(
              "Here you are able to view the heatmao of the canteen. The Data in updated every 10 seconds.",
              style: TextStyle(fontSize: 16, color: Colors.white70),
            ),
            SizedBox(height: 150),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text("Heatmap", style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold, color: Colors.white)),
                // Add timestamp display
                if (_latestTimestamp != null)
                  Text(
                    "Last updated: ${DateTime.fromMillisecondsSinceEpoch(_latestTimestamp!).toLocal().toString().substring(0, 19)}",
                    style: TextStyle(fontSize: 14, color: Colors.white70),
                  ),
              ],
            ),
            SizedBox(height: 0),
            Expanded(
              child: Center(
                child: Container(
                  width: 500,
                  height: 500,
                  color: Colors.grey[300],
                  child: _latestTimestamp == null
                      ? Icon(Icons.restaurant, size: 200, color: Colors.black54)
                      : FutureBuilder<Image?>(
                          // Use key to force refresh when _needsRefresh is true
                          key: _needsRefresh ? UniqueKey() : _futureBuilderKey,
                          future: _fetchHeatmapImage(_latestTimestamp!),
                          builder: (context, snapshot) {
                            if (snapshot.connectionState == ConnectionState.waiting) {
                              return Center(child: CircularProgressIndicator());
                            } else if (snapshot.hasError) {
                              return Center(child: Text('Error loading heatmap: ${snapshot.error}', style: TextStyle(color: Colors.red)));
                            } else if (snapshot.hasData && snapshot.data != null) {
                              return snapshot.data!;
                            } else {
                              return Icon(Icons.error_outline, size: 200, color: Colors.red);
                            }
                          },
                        ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}