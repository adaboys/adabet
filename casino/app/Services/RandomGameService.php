<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   RandomGameService.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Services;

use App\Models\User;

interface RandomGameService
{
    
    public static function createRandomGame(User $user): void;
}
