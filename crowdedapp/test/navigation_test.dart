import 'package:flutter_test/flutter_test.dart';
import 'package:crowdedapp/crowded_app.dart';
import 'package:flutter/material.dart';

void main() {
  testWidgets('Bottom navigation switches pages', (WidgetTester tester) async {
    await tester.runAsync(() async {
      await tester.pumpWidget(const Crowdedapp());

      // Home page should be visible
      expect(find.text('Welcome to Horizon'), findsOneWidget);

      // Tap the Canteen tab
      await tester.tap(find.byIcon(Icons.restaurant));
      await tester.pump(const Duration(seconds: 1)); // Use a fixed pump duration instead of pumpAndSettle

      // Canteen page should be visible
      expect(find.text('Canteen View'), findsOneWidget);
    });
  });
}