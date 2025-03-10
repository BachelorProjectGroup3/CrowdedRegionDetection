import 'package:flutter/material.dart';

class CanteenPage extends StatelessWidget {
  final String title;
  const CanteenPage({super.key, required this.title});

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
                  height: 500,
                  color: Colors.grey[300],
                  child: Icon(Icons.restaurant, size: 200, color: Colors.black54),
                ),
              ),
            )
          ],
        ),
      ),
    );
  }
}