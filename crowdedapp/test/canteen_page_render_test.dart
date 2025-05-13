import 'package:flutter_test/flutter_test.dart';
import 'package:crowdedapp/canteen_pages.dart';
import 'package:flutter/material.dart';

void main() {
  testWidgets('CanteenPage renders without crashing', (WidgetTester tester) async {
    await tester.runAsync(() async {
      await tester.pumpWidget(const MaterialApp(home: CanteenPage(title: 'Canteen View')));
      // Wait for any pending timers to complete (simulate enough time for all timers)
      await Future.delayed(const Duration(seconds: 6));
      await tester.pumpAndSettle();
      // Just check that the widget tree contains CanteenPage
      expect(find.byType(CanteenPage), findsOneWidget);
    });
  });
}