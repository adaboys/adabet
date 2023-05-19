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

use Illuminate\Database\Eloquent\Model;

class SocialProfile extends Model
{
    use StandardDateFormat;

    
    protected $fillable = [
        'user_id', 'provider_name', 'provider_user_id',
    ];

    public function user()
    {
        return $this->belongsTo(User::class);
    }
}
