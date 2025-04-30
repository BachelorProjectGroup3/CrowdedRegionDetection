import 'package:flutter_test/flutter_test.dart';
import 'package:crowdedapp/canteen_pages.dart';
import 'package:flutter/material.dart';

void main() {
  testWidgets('CanteenPage shows loading indicator', (WidgetTester tester) async {
    await tester.pumpWidget(const MaterialApp(home: CanteenPage(title: 'Canteen View')));

    // Should show a CircularProgressIndicator while loading
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
  });
}