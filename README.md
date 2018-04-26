# MyriamBot
Simple chat bot, that can recognize your face using microsoft cognitive services. More a playground project to experiment a little bit with technology

## Get Started:
1. Clone this repository
2. Get yourself an api key for the face api on https://azure.microsoft.com/en-us/try/cognitive-services/?api=face-api
3. Copy your obtained key into App.config (only first key is used for now)
4. Run and talk to the bot. For example say "Hi" to start..

## Some ideas to implement
- "What's the weather today?"
	- For example use an online service that provide this information by doing a rest call
	- Extra:
		- Can you also give a specific place?
		- Can you do it based on the location of the client?
- "Can you guess my age?"
	- Extend the face api functionality to retrieve the age of the person
- "Do you think I should shave?"
	- If the person has no beard or moustache. "Why should you? You don't have a beard or moustache"
	- If the person only have a moustache. "Yeah, shave the moustache or grow the beard"
	- ...
- "Do you think I look {emotion}?"
	- Extend the face api functionality to retrieve the emotions of the person.
	- You can answer yes if the emotion given is the most accurate one
	- Or respond no you look more {most_acurate_emotion} to me

## Ideas to investigate in future (also more time consuming than previous):
- Use LUIS instead of own custom implementation to interpet the intents of a sentence
- Use speech recognition instead of typing only
- Use bing autocorrect service to fix possible typos on the fly


