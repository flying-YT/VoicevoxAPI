using VoicevoxAPI;

// Main method
VoicevoxFunction voicevoxFunction = new("192.168.0.5", "50021");

await voicevoxFunction.MakeSound("sample", "こんにちは世界", true, 1);
