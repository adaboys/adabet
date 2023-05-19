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

use App\Models\GameRoom;
use App\Models\GameRoomPlayer;
use App\Models\User;
use Illuminate\Contracts\Validation\Rule;

class MaxOpenGameRoomsLimit implements Rule
{
    private $user;

    
    public function __construct(User $user)
    {
        $this->user = $user;
    }

    
    public function passes($attribute, $value)
    {
        return GameRoom::where('user_id', $this->user->id)
                ->open()
                ->count() < (int) config('settings.games.multiplayer.rooms_creation_limit');
    }

    
    public function message()
    {
        return __('You can have no more than :n open rooms at the same time.', ['n' => config('settings.games.multiplayer.rooms_creation_limit')]);
    }
}
