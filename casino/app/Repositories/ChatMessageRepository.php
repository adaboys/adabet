<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Repositories;

use App\Events\ChatMessageSent;
use App\Models\ChatMessage;
use App\Models\ChatRoom;
use App\Models\User;

class ChatMessageRepository
{
    public function create(ChatRoom $room, User $sender, string $message, array $recipients = [])
    {
        $chatMessage = new ChatMessage();
        $chatMessage->room()->associate($room);
        $chatMessage->user()->associate($sender);
        $chatMessage->message = $message;
        $chatMessage->save();

        if (!empty($recipients)) {
            $chatMessage->recipients()->attach($recipients);
        }

        broadcast(new ChatMessageSent($room, $chatMessage));

        return $chatMessage;
    }
}
