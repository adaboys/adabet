<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */
namespace App\Http\Requests\Admin;

use App\Models\User;
use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Validation\Rule;

class UpdateUser extends FormRequest
{
    
    public function authorize()
    {
        return TRUE;
    }

    
    public function rules()
    {
        return [
            'name'      => 'required|min:3|max:100|unique:users,name,' . $this->user->id . ',id', 
            'email'     => 'required|email|max:100|unique:users,email,' . $this->user->id . ',id', 
            'password'  => 'nullable|string|min:6',
            'role' => [
                'required',
                Rule::in(array_keys(User::roles())),
            ],
            'status' => [
                'required',
                Rule::in(array_keys(User::statuses())),
            ],
            'permissions' => 'nullable|array',
            'banned_from_chat' => 'required|boolean'
        ];
    }
}
