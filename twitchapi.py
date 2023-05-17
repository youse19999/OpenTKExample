#access_token=6paph5f2w9grecgke5zivg840q73vj&scope=&token_type=bearer
from twitchAPI.twitch import Twitch
from twitchAPI.oauth import UserAuthenticator
from twitchAPI.types import AuthScope, ChatEvent
from twitchAPI.chat import Chat, EventData, ChatMessage, ChatSub, ChatCommand
from twitchAPI.helper import first
import asyncio
APP_ID = ''
APP_SECRET = ''
USER_SCOPE = [AuthScope.CHAT_READ, AuthScope.CHAT_EDIT]
TARGET_CHANNELS = []
async def on_ready(ready_event: EventData):
    print('ボット開始')
    for target in TARGET_CHANNELS:
        await ready_event.chat.join_room(target)
async def on_message(msg: ChatMessage):
    print(f'{msg.room.name}のチャンネルで、{msg.user.name}が発言しました。コメント{msg.text}')
async def on_sub(sub: ChatSub):
    print(f'サブスクライブを開始しました。{sub.room.name}:|'
          f'プランは、{sub.sub_plan}です。|'
          f'メッセージは、{sub.sub_message}')
async def twitch_example():
    twitch = await Twitch('yihws5xo47azw0uwm9780r7rqz0ij8','oc0p7rakf7h5juqgn9j633ltevgrka')
    auth = UserAuthenticator(twitch, USER_SCOPE)
    token, refresh_token = await auth.authenticate()
    await twitch.set_user_authentication(token, USER_SCOPE, refresh_token)
    chat = await Chat(twitch)
    chat.register_event(ChatEvent.READY, on_ready)
    chat.register_event(ChatEvent.MESSAGE, on_message)
    chat.register_event(ChatEvent.SUB, on_sub)
    chat.start()
    try:
        input('press ENTER to stop\n')
    finally:
        chat.stop()
        await twitch.close()
asyncio.run(twitch_example())
