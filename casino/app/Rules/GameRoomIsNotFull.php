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
use Illuminate\Contracts\Validation\Rule;

class GameRoomIsNotFull implements Rule
{
    private $room;

    
    public function __construct(GameRoom $room)
    {
        $this->room = $room;
    }

    
    public function passes($attribute, $value)
    {
        return $this->room->players->count() < (int) $this->room->parameters->players_count;
    }

    
    public function message()
    {
        return __('The game room is already full.');
    }
}
