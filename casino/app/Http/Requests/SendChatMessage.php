<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Requests;

use App\Models\User;
use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Validation\Rule;

class SendChatMessage extends FormRequest
{
    
    public function authorize()
    {
        return $this->room->enabled && !$this->user()->banned_from_chat;
    }

    
    public function rules()
    {
        return [
            'message' => 'required|min:1|max:' . config('settings.interface.chat.message_max_length'),
            'recipients' => 'nullable|array',
            'recipients.*' => [
                'required',
                'integer',
                Rule::exists('users', 'id')->where(function ($query) {
                    $query->where('status', User::STATUS_ACTIVE);
                }),
            ]
        ];
    }
}
