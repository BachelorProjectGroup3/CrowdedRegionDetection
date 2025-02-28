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
    primaryColor: const Color.fromARGB(255, 4, 0, 255),
    scaffoldBackgroundColor: const Color.fromARGB(255, 255, 0, 0),
    colorScheme: ColorScheme.fromSeed(
      seedColor: const Color.fromARGB(255, 21, 252, 0),
      primary: const Color.fromARGB(255, 4, 0, 255),
      secondary: const Color.fromARGB(255, 255, 0, 0),
      tertiary: const Color.fromARGB(255, 0, 255, 0),
    ),

  );

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: titleString,
      theme: baseTheme,
      home: Home(title: titleString),
      );
  }
}