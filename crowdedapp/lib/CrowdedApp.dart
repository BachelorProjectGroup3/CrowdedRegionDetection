import 'package:crowdedapp/home.dart';
import 'package:flutter/material.dart';

class Crowdedapp extends StatefulWidget {
  const Crowdedapp({super.key});

  @override
  State<Crowdedapp> createState() => _CrowdedappState();
}

class _CrowdedappState extends State<Crowdedapp> {
  final String titleString = "Crowded Region Detection";

  final ThemeData baseTheme = ThemeData(
    primaryColor: const Color.fromARGB(183, 255, 218, 84),
    scaffoldBackgroundColor: const Color.fromARGB(255, 255, 234, 159)
  );

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: titleString,
      theme: baseTheme,
      home: Home(title: titleString, baseTheme: baseTheme),
      );
  }
}