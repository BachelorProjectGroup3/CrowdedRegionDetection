import 'package:flutter_test/flutter_test.dart';
import 'package:crowdedapp/crowded_app.dart';
import 'package:flutter/material.dart';

void main () {
  testWidgets('App displays bottom navigation bar', (WidgetTester tester) async {
    await tester.pumpWidget(const Crowdedapp());

    // Check if the bottom navigation bar is present
    expect(find.byType(BottomNavigationBar), findsOneWidget);

    // Check if the Home and Canteen tabs are present
    expect(find.text('Home'), findsOneWidget);
    expect(find.text('Canteen'), findsOneWidget);
  });
}
