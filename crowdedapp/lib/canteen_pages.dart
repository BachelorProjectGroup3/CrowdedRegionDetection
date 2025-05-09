import 'dart:async';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:flutter_dotenv/flutter_dotenv.dart';
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
  // Track when the image was last updated
  DateTime? _lastUpdated;
  bool _needsRefresh = false;
  Image? _lastImage;

  @override
  void initState() {
    super.initState();
    _needsRefresh = true; // Ensure image loads on first open
    _initSignalR();
  }

  Future<void> _initSignalR() async {
    try {
      print('Initializing SignalR connection to: $backendUrl/hubs/detecteddevices');
      if (_hubConnection == null || _hubConnection?.state == HubConnectionState.disconnected) {
        _hubConnection = HubConnectionBuilder()
            .withUrl(
              '$backendUrl/hubs/detecteddevices',
              HttpConnectionOptions(
                logging: (level, message) => print('SignalR $level: $message'),
              ),
            )
            .withAutomaticReconnect()
            .build();

        _hubConnection?.onreconnecting((error) {
          print('SignalR reconnecting due to error: \\${error?.toString() ?? "Unknown error"}');
        });
        _hubConnection?.onreconnected((connectionId) {
          print('SignalR reconnected with connectionId: $connectionId');
        });
        _hubConnection?.onclose((error) {
          print('SignalR connection closed: \\${error?.toString() ?? "No error"}');
          Future.delayed(Duration(seconds: 3), () {
            if (mounted) {
              _initSignalR();
            }
          });
        });
        _hubConnection?.on('NewDevicesDetected', (arguments) async {
          print('SignalR: NewDevicesDetected event received!');
          print('Jeg bliver ikke mounted');
          if (mounted) {
            print('Jeg skal opdatere heatmap');
            setState(() {
              _needsRefresh = true;
            });
            // Optionally, immediately trigger a fetch so the UI updates as soon as possible
            await _fetchLatestHeatmapImage();
          }
        });
      }
      if (_hubConnection?.state != HubConnectionState.connected) {
        print('Starting connection to SignalR hub...');
        await _hubConnection?.start();
        print('Connected to SignalR hub successfully!');
      }
    } catch (e) {
      print('Error initializing SignalR: $e');
      if (mounted) {
        Future.delayed(Duration(seconds: 5), () {
          if (mounted && (_hubConnection == null ||
              _hubConnection?.state == HubConnectionState.disconnected ||
              _hubConnection?.state == HubConnectionState.disconnecting)) {
            print('Attempting to reconnect after error...');
            _initSignalR();
          }
        });
      }
    }
  }

  // Fetch the latest valid heatmap image from the new endpoint
  Future<Image?> _fetchLatestHeatmapImage() async {
    // Add a short delay to ensure backend has generated the new heatmap
    await Future.delayed(const Duration(milliseconds: 500));
    try {
      final response = await http.get(
        Uri.parse('$backendUrl/api/DetectedDevices/getLatestValidHeatmap'),
      );
      if (response.statusCode == 200) {
        final bytes = base64Decode(response.body);
        if (bytes.isEmpty) {
          print('Error: Received empty image data');
          return null;
        }
        final image = Image.memory(bytes, fit: BoxFit.contain);
        if (mounted) {
          print('Jeg kommer her ind');
          setState(() {
            _lastImage = image;
            _lastUpdated = DateTime.now();
            _needsRefresh = false;
          });
        }
        return image;
      } else {
        print('Error fetching heatmap: Status \\${response.statusCode}');
        return null;
      }
    } catch (e) {
      print('Error fetching heatmap: $e');
      return null;
    }
  }

  @override
  void dispose() {
    try {
      _hubConnection?.stop().catchError((error) {
        print("Error stopping connection: $error");
      });
    } catch (e) {
      print("Exception while stopping connection: $e");
    }
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
              "Here you are able to view the heatmao of the canteen. The Data is updated every 10 seconds.",
              style: TextStyle(fontSize: 16, color: Colors.white70),
            ),
            SizedBox(height: 150),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text("Heatmap", style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold, color: Colors.white)),
                // Show last updated time if available
                if (_lastUpdated != null)
                  Text(
                    "Last updated: \\${_lastUpdated!.toLocal().toString().substring(0, 19)}",
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
                  color: Colors.transparent,
                  child: _needsRefresh
                      ? FutureBuilder<Image?>(
                          key: UniqueKey(),
                          future: _fetchLatestHeatmapImage(),
                          builder: (context, snapshot) {
                            if (snapshot.connectionState == ConnectionState.waiting) {
                              return Center(child: CircularProgressIndicator());
                            } else if (snapshot.hasError) {
                              return Center(child: Text('Error loading heatmap: \\${snapshot.error}', style: TextStyle(color: Colors.red)));
                            } else if (snapshot.hasData && snapshot.data != null) {
                              return snapshot.data!;
                            } else {
                              return Icon(Icons.error_outline, size: 200, color: Colors.red);
                            }
                          },
                        )
                      : (_lastImage ?? Icon(Icons.restaurant, size: 200, color: Colors.black54)),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}