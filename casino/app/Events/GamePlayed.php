<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Events;

use App\Models\Game;
use Illuminate\Broadcasting\Channel;
use Illuminate\Contracts\Broadcasting\ShouldBroadcastNow;
use Illuminate\Queue\SerializesModels;
use Illuminate\Broadcasting\PrivateChannel;
use Illuminate\Broadcasting\PresenceChannel;
use Illuminate\Foundation\Events\Dispatchable;
use Illuminate\Broadcasting\InteractsWithSockets;

class GamePlayed implements ShouldBroadcastNow
{
    use Dispatchable, InteractsWithSockets, SerializesModels;

    public $game;

    
    public function __construct(Game $game)
    {
        $this->game = $game;
    }

    
    public function broadcastOn()
    {
        return new PrivateChannel('games');
    }

    
    public function broadcastWhen()
    {
        return config('broadcasting.connections.pusher.app_id')
            && config('broadcasting.connections.pusher.key')
            && config('broadcasting.connections.pusher.secret')
            && config('settings.interface.game_feed.enabled');
    }

    
    public function broadcastWith()
    {
        $game = $this->game->only('id', 'bet', 'win', 'title', 'created_ago');
        $game['account'] = ['user' => $this->game->account->user->only('id', 'name', 'avatar_url')];

        return $game;
    }
}
