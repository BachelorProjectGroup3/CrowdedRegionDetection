import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:flutter_dotenv/flutter_dotenv.dart';
final String backendUrl = dotenv.env['BackendURL'] ?? 'http://localhost:3000';
class CanteenPage extends StatelessWidget {
  final String title;
  const CanteenPage({super.key, required this.title});
  

Future<Image?> fetchHeatmapImage(int timestamp) async {
  final response = await http.get(
    Uri.parse('$backendUrl/api/DetectedDevices/getHeatmapAtSpecificTime/$timestamp'),
  );
  try {
    if (response.statusCode == 200) {
      final bytes = base64Decode(response.body);
      return Image.memory(bytes, fit: BoxFit.contain);
    } else {
      throw Exception('Failed to load heatmap image');
    }
  } catch (e) {
    print('Error fetching heatmap image: $e');
  }
  return null;
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
              title,
              style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold, color: Colors.white),
            ),
            SizedBox(height: 10),
            Text(
              "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean vel dui diam. Nulla facilisi.",
              style: TextStyle(fontSize: 16, color: Colors.white70),
            ),
            SizedBox(height: 150),
            Text("Heatmap", style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold, color: Colors.white)),
            SizedBox(height: 0),
            Expanded(
              child: Center(
                child: Container(
                  width: 500,
                  height: 400,
                  color: Colors.grey[300],
                  child: FutureBuilder<Image?>(
                    future: fetchHeatmapImage(1745916000000), // Replace with your timestamp
                    builder: (context, snapshot) {
                      if (snapshot.connectionState == ConnectionState.waiting) {
                        return Center(child: CircularProgressIndicator());
                      } else if (snapshot.hasError) {
                        return Text('Error: ${snapshot.error}');
                      } else if (snapshot.hasData && snapshot.data != null) {
                        return snapshot.data!;
                      } else {
                        return Icon(Icons.restaurant, size: 200, color: Colors.black54);
                      }
                    },
                  ),
                ),
              ),
            )
          ],
        ),
      ),
    );
  }
}