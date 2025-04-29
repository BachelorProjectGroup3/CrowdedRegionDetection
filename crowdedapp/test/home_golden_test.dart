import 'package:flutter_test/flutter_test.dart';
import 'package:flutter/material.dart';
import 'package:crowdedapp/home.dart';

void main() {
  testWidgets('Home widget matches golden file', (WidgetTester tester) async {
    await tester.pumpWidget(
      const MaterialApp(
        home: Scaffold(body: Home()),
      ),
    );
    await expectLater(
      find.byType(Home),
      matchesGoldenFile('goldens/home_golden.png'),
    );
  });
}