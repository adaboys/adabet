<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */


namespace App\Models;

interface ProvableGame
{
    
    public function getSecretAttributeAttribute(): string;

    
    public function getSecretAttributeHintAttribute(): string;
}
