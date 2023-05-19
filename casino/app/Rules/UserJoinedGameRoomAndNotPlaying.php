<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */
namespace App\Rules;

use App\Models\GameRoomPlayer;
use App\Models\User;
use Illuminate\Contracts\Validation\Rule;

class UserJoinedGameRoomAndNotPlaying implements Rule
{
    private $user;

    
    public function __construct(User $user)
    {
        $this->user = $user;
    }

    
    public function passes($attribute, $value)
    {
        $player = GameRoomPlayer::where('game_room_id', $value)->where('user_id', $this->user->id)->first();

        
        return $player && !$player->game_id;
    }

    
    public function message()
    {
        return __('You can not leave this room.');
    }
}
