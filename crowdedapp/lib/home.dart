import 'package:flutter/material.dart';

class Home extends StatefulWidget {
  const Home({super.key, required this.title, required this.baseTheme});

  final String title;
  final ThemeData baseTheme;

  @override
  State<Home> createState() => _HomeState();
}

class _HomeState extends State<Home> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.title),
        backgroundColor: widget.baseTheme.primaryColor,
      ),
    );
  }
}