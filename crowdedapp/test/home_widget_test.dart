import 'package:flutter_test/flutter_test.dart';
import 'package:crowdedapp/home.dart';
import 'package:flutter/material.dart';

void main() {
  testWidgets('Home page displays welcome text and description', (WidgetTester tester) async {
    await tester.pumpWidget(const MaterialApp(home: Home()));

    expect(find.text('Welcome to Horizon'), findsOneWidget);
    expect(find.text('Get real-time insights on crowded areas and navigate smarter.'), findsOneWidget);
  });
}