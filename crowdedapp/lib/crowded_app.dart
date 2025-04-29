// ignore: file_names
import 'package:crowdedapp/home.dart';
import 'package:crowdedapp/canteen_pages.dart';
import 'package:flutter/material.dart';

class Crowdedapp extends StatefulWidget {
  const Crowdedapp({super.key});

  @override
  State<Crowdedapp> createState() => _CrowdedappState();
}

class _CrowdedappState extends State<Crowdedapp> {
  int _currentIndex = 0;
  final List<Widget> _pages = [
    Home(),
    CanteenPage(title: 'Canteen View'),
  ];

  void _onItemTapped(int index) {
    setState(() {
      _currentIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: Scaffold(
        body: _pages[_currentIndex],
        bottomNavigationBar: BottomNavigationBar(
          currentIndex: _currentIndex,
          onTap: _onItemTapped,
          backgroundColor: Colors.blue.shade600,
          selectedItemColor: Colors.black,
          unselectedItemColor: Colors.white,
          items: [
            BottomNavigationBarItem(icon: Icon(Icons.home), label: "Home"),
            BottomNavigationBarItem(icon: Icon(Icons.restaurant), label: "Canteen"),
          ],
        ),
      ),
    );
  }
}